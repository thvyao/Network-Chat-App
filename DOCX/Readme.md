# Thiết kế kiến trúc hệ thống

> Tài liệu này mô tả thiết kế của phiên bản nền tảng: kết nối TCP, đăng ký/đăng nhập, quản lý người dùng trực tuyến và định tuyến tin nhắn. Các tính năng Reply, Forward và Emoji dùng chung phong bì `ChatMessage` và sẽ được hoàn thiện ở các tuần sau.

## 1. Sơ đồ kiến trúc tổng thể

```mermaid
flowchart LR
    A[Người dùng A] --> CA[ChatClient<br/>WinForms]
    B[Người dùng B] --> CB[ChatClient<br/>WinForms]

    subgraph client[Ứng dụng Client]
        CA --> NCA[ChatClientConnection<br/>Receive loop + events]
        CB --> NCB[ChatClientConnection<br/>Receive loop + events]
    end

    NCA <-->|TCP / JSON UTF-8<br/>length-prefixed ChatMessage| S
    NCB <-->|TCP / JSON UTF-8<br/>length-prefixed ChatMessage| S

    subgraph server[Ứng dụng ChatServer]
        S[ServerForm<br/>TcpListener] --> H[Task HandleClient<br/>mỗi kết nối]
        H --> R[Đăng nhập & định tuyến]
        R --> O[(ConcurrentDictionary<br/>connectedClients)]
        R --> AM[AccountManager]
        AM --> AR[TextAccountRepository]
        AR --> DB[(accounts.txt)]
    end

    subgraph shared[Thư viện ChatShared]
        P[ChatMessage + MessageType]
        JS[JsonChatMessageSerializer]
    end

    NCA -. sử dụng .-> P
    NCA -. sử dụng .-> JS
    NCB -. sử dụng .-> P
    NCB -. sử dụng .-> JS
    R -. sử dụng .-> P
    R -. sử dụng .-> JS
```

**Nguyên tắc:** Client không giao tiếp trực tiếp với nhau. Mọi tin nhắn luôn đi qua Server để xác thực người gửi, chọn người nhận hoặc broadcast, sau đó Server mới gửi lại cho các Client phù hợp.

## 2. Class Diagram

```mermaid
classDiagram
    class ChatMessage {
        +string Id
        +MessageType Type
        +string Sender
        +string Recipient
        +string Content
        +DateTime Timestamp
        +string ReplyToId
        +string ReplyToContent
        +string ReplyToSender
        +string ForwardFrom
        +string[] UserList
        +bool Success
        +int AvatarIndex
        +string Password
    }

    class MessageType {
        <<enumeration>>
        Login
        Register
        TextMessage
        PrivateMessage
        ReplyMessage
        ForwardMessage
        EmojiMessage
        UserList
        Error
    }

    class IChatMessageSerializer {
        <<interface>>
        +Serialize(ChatMessage) string
        +Deserialize(string) ChatMessage
        +SendAsync(NetworkStream, ChatMessage)
        +ReceiveAsync(NetworkStream) ChatMessage
    }

    class JsonChatMessageSerializer
    class IChatConnection {
        <<interface>>
        +bool IsConnected
        +MessageReceived
        +ConnectionClosed
        +SendAsync(ChatMessage)
        +StartReceiving()
        +Disconnect()
    }
    class ChatClientConnection {
        -TcpClient client
        -NetworkStream stream
        -IChatMessageSerializer serializer
        +SendAsync(ChatMessage)
        +StartReceiving()
        +Disconnect()
    }
    class ClientForm {
        -IChatConnection chatConnection
        +ConnectToServer()
        +ProcessReceivedMessage(ChatMessage)
        +SendTextMessage()
    }

    class ServerForm {
        -TcpListener listener
        -ConcurrentDictionary connectedClients
        -AccountManager accountManager
        +StartServer()
        +HandleClient(TcpClient)
        +BroadcastMessage(ChatMessage)
        +SendToClient(ClientInfo, ChatMessage)
    }
    class ClientInfo {
        +TcpClient TcpClient
        +NetworkStream Stream
        +string Username
        +int AvatarIndex
        +SemaphoreSlim SendLock
    }
    class AccountManager {
        -IAccountRepository repository
        +Register(string, string, int) bool
        +VerifyLogin(string, string) bool
    }
    class IAccountRepository {
        <<interface>>
        +Load() IReadOnlyList~UserAccount~
        +Save(IEnumerable~UserAccount~)
    }
    class TextAccountRepository
    class UserAccount {
        +string Username
        +string PasswordHash
        +string Salt
        +int AvatarIndex
    }

    ChatMessage --> MessageType
    JsonChatMessageSerializer ..|> IChatMessageSerializer
    ChatClientConnection ..|> IChatConnection
    ChatClientConnection --> IChatMessageSerializer
    ClientForm --> IChatConnection
    ServerForm --> ClientInfo
    ServerForm --> AccountManager
    AccountManager --> IAccountRepository
    TextAccountRepository ..|> IAccountRepository
    AccountManager --> UserAccount
```

## 3. Sequence Diagram — Đăng nhập và gửi tin nhắn công khai
Sơ đồ mô tả luồng đăng nhập của người dùng A và việc gửi một tin nhắn công khai (broadcast) qua Server. Có hai nhánh: đăng nhập hợp lệ và không hợp lệ.
```mermaid
sequenceDiagram
    autonumber
    actor U as Người dùng A
    participant UI as ClientForm
    participant CC as ChatClientConnection
    participant S as ServerForm
    participant AM as AccountManager
    participant O as connectedClients
    participant B as Client B

    U->>UI: Nhập IP, port, tài khoản và bấm Đăng nhập
    UI->>S: TCP connect
    UI->>S: ChatMessage(Login, username, password)
    S->>AM: VerifyLogin(username, password)
    AM-->>S: Hợp lệ / không hợp lệ
    alt Đăng nhập hợp lệ
        S->>O: Thêm ClientInfo theo username
        S-->>UI: LoginResult(success=true, userList)
        S-->>B: UserJoined + UserList
        UI->>CC: StartReceiving() và đăng ký event
        U->>UI: Nhập nội dung và bấm Gửi
        UI->>CC: SendAsync(TextMessage)
        CC->>S: ChatMessage(TextMessage)
        S->>S: Xác thực sender, kiểm tra độ dài/nội dung
        S-->>UI: BroadcastMessage(TextMessage)
        S-->>B: BroadcastMessage(TextMessage)
        CC-->>UI: MessageReceived
        UI-->>U: Hiển thị MessageBubble
    else Đăng nhập không hợp lệ
        S-->>UI: LoginResult(success=false, lỗi)
        UI-->>U: Hiển thị thông báo lỗi
    end
```
Giải thích ngắn
Bước	Mô tả
1–5	Client kết nối TCP, gửi `Login`; Server nhờ `AccountManager` xác thực tài khoản.
6–17 (nhánh hợp lệ)	Server lưu `ClientInfo`, báo cho Client B, Client A bắt đầu nhận tin; A gửi tin, Server kiểm tra rồi broadcast cho A và B, giao diện hiển thị `MessageBubble`.
18–19 (nhánh không hợp lệ)	Server trả `LoginResult(success=false)`, Client hiển thị lỗi.
