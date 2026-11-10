namespace ChatClient
{
    partial class ClientForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ClientForm));
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this.pnlUsersHeader = new System.Windows.Forms.Panel();
            this.lblUsersHeader = new System.Windows.Forms.Label();
            this.pnlNavigation = new System.Windows.Forms.Panel();
            this.btnNewGroup = new System.Windows.Forms.Button();
            this.btnGroups = new System.Windows.Forms.Button();
            this.btnChats = new System.Windows.Forms.Button();
            this.pnlSearch = new System.Windows.Forms.Panel();
            this.lblSearchIcon = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.pnlBrand = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.lblBrandName = new System.Windows.Forms.Label();
            this.picBrandLogo = new System.Windows.Forms.PictureBox();
            this.lstUsers = new System.Windows.Forms.ListBox();
            this.pnlUserProfile = new System.Windows.Forms.Panel();
            this.btnSettings = new System.Windows.Forms.Button();
            this.lblOwnStatus = new System.Windows.Forms.Label();
            this.lblOwnStatusDot = new System.Windows.Forms.Label();
            this.lblOwnUsername = new System.Windows.Forms.Label();
            this.picOwnAvatar = new System.Windows.Forms.PictureBox();
            this.pnlSettingsView = new System.Windows.Forms.Panel();
            this.pnlSettingsFooter = new System.Windows.Forms.Panel();
            this.pnlSettingsBody = new System.Windows.Forms.Panel();
            this.pnlOptions = new System.Windows.Forms.Panel();
            this.chkNotificationSound = new System.Windows.Forms.CheckBox();
            this.chkNotifications = new System.Windows.Forms.CheckBox();
            this.lblOptionsTitle = new System.Windows.Forms.Label();
            this.tblSettingsCards = new System.Windows.Forms.TableLayoutPanel();
            this.pnlAccountInfo = new System.Windows.Forms.Panel();
            this.lblAccountSectionTitle = new System.Windows.Forms.Label();
            this.lblAccountUsernameCaption = new System.Windows.Forms.Label();
            this.lblAccountUsernameValue = new System.Windows.Forms.Label();
            this.lblAccountStatusCaption = new System.Windows.Forms.Label();
            this.lblAccountStatusValue = new System.Windows.Forms.Label();
            this.pnlConnectionInfo = new System.Windows.Forms.Panel();
            this.lblConnectionSectionTitle = new System.Windows.Forms.Label();
            this.lblServerIPCaption = new System.Windows.Forms.Label();
            this.lblServerIPValue = new System.Windows.Forms.Label();
            this.lblPortCaption = new System.Windows.Forms.Label();
            this.lblPortValue = new System.Windows.Forms.Label();
            this.lblConnectionStatusCaption = new System.Windows.Forms.Label();
            this.lblConnectionStatusValue = new System.Windows.Forms.Label();
            this.pnlSettingsHeader = new System.Windows.Forms.Panel();
            this.btnBackToChat = new System.Windows.Forms.Button();
            this.lblSettingsSubtitle = new System.Windows.Forms.Label();
            this.lblSettingsTitle = new System.Windows.Forms.Label();
            this.pnlChatFlow = new System.Windows.Forms.FlowLayoutPanel();
            this.pnlReplyPreview = new System.Windows.Forms.Panel();
            this.btnCancelReply = new System.Windows.Forms.Button();
            this.lblReplyPreviewText = new System.Windows.Forms.Label();
            this.pnlReplyBar = new System.Windows.Forms.Panel();
            this.pnlInput = new System.Windows.Forms.Panel();
            this.btnSend = new System.Windows.Forms.Button();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.btnEmoji = new System.Windows.Forms.Button();
            this.pnlChatHeader = new System.Windows.Forms.Panel();
            this.pnlChatActions = new System.Windows.Forms.Panel();
            this.btnHeaderMore = new System.Windows.Forms.Button();
            this.btnHeaderVideo = new System.Windows.Forms.Button();
            this.btnHeaderCall = new System.Windows.Forms.Button();
            this.btnHeaderSearch = new System.Windows.Forms.Button();
            this.lblChatSub = new System.Windows.Forms.Label();
            this.lblCurrentChat = new System.Windows.Forms.Label();
            this.picChatHeaderAvatar = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).BeginInit();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.Panel2.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            this.pnlUsersHeader.SuspendLayout();
            this.pnlNavigation.SuspendLayout();
            this.pnlSearch.SuspendLayout();
            this.pnlBrand.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picBrandLogo)).BeginInit();
            this.pnlUserProfile.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picOwnAvatar)).BeginInit();
            this.pnlSettingsView.SuspendLayout();
            this.pnlSettingsBody.SuspendLayout();
            this.pnlOptions.SuspendLayout();
            this.tblSettingsCards.SuspendLayout();
            this.pnlAccountInfo.SuspendLayout();
            this.pnlConnectionInfo.SuspendLayout();
            this.pnlSettingsHeader.SuspendLayout();
            this.pnlReplyPreview.SuspendLayout();
            this.pnlInput.SuspendLayout();
            this.pnlChatHeader.SuspendLayout();
            this.pnlChatActions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picChatHeaderAvatar)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainerMain
            // 
            this.splitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainerMain.IsSplitterFixed = true;
            this.splitContainerMain.Location = new System.Drawing.Point(0, 0);
            this.splitContainerMain.Name = "splitContainerMain";
            // 
            // splitContainerMain.Panel1
            // 
            this.splitContainerMain.Panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(36)))), ((int)(((byte)(140)))));
            this.splitContainerMain.Panel1.Controls.Add(this.pnlUsersHeader);
            this.splitContainerMain.Panel1.Controls.Add(this.pnlNavigation);
            this.splitContainerMain.Panel1.Controls.Add(this.pnlSearch);
            this.splitContainerMain.Panel1.Controls.Add(this.pnlBrand);
            this.splitContainerMain.Panel1.Controls.Add(this.lstUsers);
            this.splitContainerMain.Panel1.Controls.Add(this.pnlUserProfile);
            // 
            // splitContainerMain.Panel2
            // 
            this.splitContainerMain.Panel2.BackColor = System.Drawing.Color.White;
            this.splitContainerMain.Panel2.Controls.Add(this.pnlSettingsView);
            this.splitContainerMain.Panel2.Controls.Add(this.pnlChatFlow);
            this.splitContainerMain.Panel2.Controls.Add(this.pnlReplyPreview);
            this.splitContainerMain.Panel2.Controls.Add(this.pnlInput);
            this.splitContainerMain.Panel2.Controls.Add(this.pnlChatHeader);
            this.splitContainerMain.Size = new System.Drawing.Size(1100, 700);
            this.splitContainerMain.SplitterDistance = 385;
            this.splitContainerMain.SplitterWidth = 1;
            this.splitContainerMain.TabIndex = 0;
            // 
            // pnlUsersHeader
            // 
            this.pnlUsersHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(36)))), ((int)(((byte)(140)))));
            this.pnlUsersHeader.Controls.Add(this.lblUsersHeader);
            this.pnlUsersHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlUsersHeader.Location = new System.Drawing.Point(0, 215);
            this.pnlUsersHeader.Name = "pnlUsersHeader";
            this.pnlUsersHeader.Size = new System.Drawing.Size(385, 30);
            this.pnlUsersHeader.TabIndex = 1;
            // 
            // lblUsersHeader
            // 
            this.lblUsersHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblUsersHeader.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblUsersHeader.ForeColor = System.Drawing.Color.White;
            this.lblUsersHeader.Location = new System.Drawing.Point(0, 0);
            this.lblUsersHeader.Name = "lblUsersHeader";
            this.lblUsersHeader.Padding = new System.Windows.Forms.Padding(12, 6, 0, 0);
            this.lblUsersHeader.Size = new System.Drawing.Size(385, 30);
            this.lblUsersHeader.TabIndex = 0;
            this.lblUsersHeader.Text = "NGƯỜI DÙNG TRỰC TUYẾN";
            // 
            // pnlNavigation
            // 
            this.pnlNavigation.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlNavigation.Controls.Add(this.btnNewGroup);
            this.pnlNavigation.Controls.Add(this.btnGroups);
            this.pnlNavigation.Controls.Add(this.btnChats);
            this.pnlNavigation.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlNavigation.Location = new System.Drawing.Point(0, 160);
            this.pnlNavigation.Name = "pnlNavigation";
            this.pnlNavigation.Size = new System.Drawing.Size(385, 55);
            this.pnlNavigation.TabIndex = 5;
            // 
            // btnNewGroup
            // 
            this.btnNewGroup.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(104)))), ((int)(((byte)(64)))), ((int)(((byte)(165)))));
            this.btnNewGroup.FlatAppearance.BorderSize = 0;
            this.btnNewGroup.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNewGroup.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.btnNewGroup.ForeColor = System.Drawing.Color.White;
            this.btnNewGroup.Location = new System.Drawing.Point(340, 10);
            this.btnNewGroup.Name = "btnNewGroup";
            this.btnNewGroup.Size = new System.Drawing.Size(40, 36);
            this.btnNewGroup.TabIndex = 2;
            this.btnNewGroup.Text = "+";
            this.btnNewGroup.UseVisualStyleBackColor = false;
            // 
            // btnGroups
            // 
            this.btnGroups.FlatAppearance.BorderSize = 0;
            this.btnGroups.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGroups.Font = new System.Drawing.Font("Segoe UI", 10.20895F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGroups.ForeColor = System.Drawing.Color.White;
            this.btnGroups.Location = new System.Drawing.Point(183, 10);
            this.btnGroups.Name = "btnGroups";
            this.btnGroups.Size = new System.Drawing.Size(151, 36);
            this.btnGroups.TabIndex = 1;
            this.btnGroups.Text = "Nhóm";
            this.btnGroups.UseVisualStyleBackColor = true;
            // 
            // btnChats
            // 
            this.btnChats.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(104)))), ((int)(((byte)(64)))), ((int)(((byte)(165)))));
            this.btnChats.FlatAppearance.BorderSize = 0;
            this.btnChats.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnChats.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnChats.ForeColor = System.Drawing.Color.White;
            this.btnChats.Location = new System.Drawing.Point(12, 10);
            this.btnChats.Name = "btnChats";
            this.btnChats.Size = new System.Drawing.Size(165, 36);
            this.btnChats.TabIndex = 0;
            this.btnChats.Text = "Trò chuyện";
            this.btnChats.UseVisualStyleBackColor = false;
            this.btnChats.Click += new System.EventHandler(this.btnChats_Click);
            // 
            // pnlSearch
            // 
            this.pnlSearch.Controls.Add(this.lblSearchIcon);
            this.pnlSearch.Controls.Add(this.txtSearch);
            this.pnlSearch.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlSearch.Location = new System.Drawing.Point(0, 100);
            this.pnlSearch.Name = "pnlSearch";
            this.pnlSearch.Size = new System.Drawing.Size(385, 60);
            this.pnlSearch.TabIndex = 4;
            // 
            // lblSearchIcon
            // 
            this.lblSearchIcon.Font = new System.Drawing.Font("Segoe UI", 15F);
            this.lblSearchIcon.ForeColor = System.Drawing.Color.White;
            this.lblSearchIcon.Location = new System.Drawing.Point(5, 8);
            this.lblSearchIcon.Name = "lblSearchIcon";
            this.lblSearchIcon.Size = new System.Drawing.Size(21, 35);
            this.lblSearchIcon.TabIndex = 1;
            this.lblSearchIcon.Text = "⌕";
            this.lblSearchIcon.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtSearch
            // 
            this.txtSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSearch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(99)))), ((int)(((byte)(60)))), ((int)(((byte)(153)))));
            this.txtSearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSearch.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtSearch.ForeColor = System.Drawing.Color.White;
            this.txtSearch.Location = new System.Drawing.Point(25, 15);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(344, 32);
            this.txtSearch.TabIndex = 0;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            // 
            // pnlBrand
            // 
            this.pnlBrand.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlBrand.Controls.Add(this.label1);
            this.pnlBrand.Controls.Add(this.lblBrandName);
            this.pnlBrand.Controls.Add(this.picBrandLogo);
            this.pnlBrand.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlBrand.Location = new System.Drawing.Point(0, 0);
            this.pnlBrand.Name = "pnlBrand";
            this.pnlBrand.Size = new System.Drawing.Size(385, 100);
            this.pnlBrand.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(84, 61);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(249, 25);
            this.label1.TabIndex = 2;
            this.label1.Text = "Mọi kết nối, một không gian";
            // 
            // lblBrandName
            // 
            this.lblBrandName.AutoSize = true;
            this.lblBrandName.BackColor = System.Drawing.Color.Transparent;
            this.lblBrandName.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblBrandName.ForeColor = System.Drawing.Color.White;
            this.lblBrandName.Location = new System.Drawing.Point(81, 26);
            this.lblBrandName.Name = "lblBrandName";
            this.lblBrandName.Size = new System.Drawing.Size(152, 46);
            this.lblBrandName.TabIndex = 1;
            this.lblBrandName.Text = "WeTALK";
            // 
            // picBrandLogo
            // 
            this.picBrandLogo.Image = ((System.Drawing.Image)(resources.GetObject("picBrandLogo.Image")));
            this.picBrandLogo.Location = new System.Drawing.Point(12, 17);
            this.picBrandLogo.Name = "picBrandLogo";
            this.picBrandLogo.Size = new System.Drawing.Size(70, 70);
            this.picBrandLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picBrandLogo.TabIndex = 0;
            this.picBrandLogo.TabStop = false;
            // 
            // lstUsers
            // 
            this.lstUsers.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(36)))), ((int)(((byte)(140)))));
            this.lstUsers.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstUsers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstUsers.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.lstUsers.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lstUsers.ForeColor = System.Drawing.Color.White;
            this.lstUsers.FormattingEnabled = true;
            this.lstUsers.ItemHeight = 58;
            this.lstUsers.Location = new System.Drawing.Point(0, 0);
            this.lstUsers.Name = "lstUsers";
            this.lstUsers.Size = new System.Drawing.Size(385, 635);
            this.lstUsers.TabIndex = 2;
            this.lstUsers.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.lstUsers_DrawItem);
            this.lstUsers.SelectedIndexChanged += new System.EventHandler(this.lstUsers_SelectedIndexChanged);
            // 
            // pnlUserProfile
            // 
            this.pnlUserProfile.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(36)))), ((int)(((byte)(140)))));
            this.pnlUserProfile.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlUserProfile.Controls.Add(this.btnSettings);
            this.pnlUserProfile.Controls.Add(this.lblOwnStatus);
            this.pnlUserProfile.Controls.Add(this.lblOwnStatusDot);
            this.pnlUserProfile.Controls.Add(this.lblOwnUsername);
            this.pnlUserProfile.Controls.Add(this.picOwnAvatar);
            this.pnlUserProfile.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlUserProfile.Location = new System.Drawing.Point(0, 635);
            this.pnlUserProfile.Name = "pnlUserProfile";
            this.pnlUserProfile.Size = new System.Drawing.Size(385, 65);
            this.pnlUserProfile.TabIndex = 0;
            // 
            // btnSettings
            // 
            this.btnSettings.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSettings.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnSettings.FlatAppearance.BorderSize = 0;
            this.btnSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSettings.Font = new System.Drawing.Font("Segoe UI Symbol", 18F);
            this.btnSettings.ForeColor = System.Drawing.Color.White;
            this.btnSettings.Location = new System.Drawing.Point(331, 0);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(52, 63);
            this.btnSettings.TabIndex = 4;
            this.btnSettings.Text = "⚙";
            this.btnSettings.UseVisualStyleBackColor = true;
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
            // 
            // lblOwnStatus
            // 
            this.lblOwnStatus.AutoSize = true;
            this.lblOwnStatus.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblOwnStatus.ForeColor = System.Drawing.Color.White;
            this.lblOwnStatus.Location = new System.Drawing.Point(72, 40);
            this.lblOwnStatus.Name = "lblOwnStatus";
            this.lblOwnStatus.Size = new System.Drawing.Size(90, 20);
            this.lblOwnStatus.TabIndex = 3;
            this.lblOwnStatus.Text = "Ngoại tuyến";
            // 
            // lblOwnStatusDot
            // 
            this.lblOwnStatusDot.AutoSize = true;
            this.lblOwnStatusDot.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblOwnStatusDot.ForeColor = System.Drawing.Color.Gray;
            this.lblOwnStatusDot.Location = new System.Drawing.Point(61, 40);
            this.lblOwnStatusDot.Name = "lblOwnStatusDot";
            this.lblOwnStatusDot.Size = new System.Drawing.Size(20, 23);
            this.lblOwnStatusDot.TabIndex = 2;
            this.lblOwnStatusDot.Text = "●";
            // 
            // lblOwnUsername
            // 
            this.lblOwnUsername.AutoSize = true;
            this.lblOwnUsername.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Bold);
            this.lblOwnUsername.ForeColor = System.Drawing.Color.White;
            this.lblOwnUsername.Location = new System.Drawing.Point(62, 18);
            this.lblOwnUsername.Name = "lblOwnUsername";
            this.lblOwnUsername.Size = new System.Drawing.Size(167, 28);
            this.lblOwnUsername.TabIndex = 1;
            this.lblOwnUsername.Text = "Chưa đăng nhập";
            // 
            // picOwnAvatar
            // 
            this.picOwnAvatar.Location = new System.Drawing.Point(12, 14);
            this.picOwnAvatar.Name = "picOwnAvatar";
            this.picOwnAvatar.Size = new System.Drawing.Size(44, 44);
            this.picOwnAvatar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picOwnAvatar.TabIndex = 0;
            this.picOwnAvatar.TabStop = false;
            // 
            // pnlSettingsView
            // 
            this.pnlSettingsView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(247)))), ((int)(((byte)(252)))));
            this.pnlSettingsView.Controls.Add(this.pnlSettingsFooter);
            this.pnlSettingsView.Controls.Add(this.pnlSettingsBody);
            this.pnlSettingsView.Controls.Add(this.pnlSettingsHeader);
            this.pnlSettingsView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlSettingsView.Location = new System.Drawing.Point(0, 70);
            this.pnlSettingsView.Name = "pnlSettingsView";
            this.pnlSettingsView.Size = new System.Drawing.Size(714, 516);
            this.pnlSettingsView.TabIndex = 0;
            // 
            // pnlSettingsFooter
            // 
            this.pnlSettingsFooter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(247)))), ((int)(((byte)(252)))));
            this.pnlSettingsFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlSettingsFooter.Location = new System.Drawing.Point(0, 444);
            this.pnlSettingsFooter.Name = "pnlSettingsFooter";
            this.pnlSettingsFooter.Padding = new System.Windows.Forms.Padding(0, 12, 24, 12);
            this.pnlSettingsFooter.Size = new System.Drawing.Size(714, 72);
            this.pnlSettingsFooter.TabIndex = 4;
            // 
            // pnlSettingsBody
            // 
            this.pnlSettingsBody.AutoScroll = true;
            this.pnlSettingsBody.AutoSize = true;
            this.pnlSettingsBody.Controls.Add(this.pnlOptions);
            this.pnlSettingsBody.Controls.Add(this.tblSettingsCards);
            this.pnlSettingsBody.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlSettingsBody.Location = new System.Drawing.Point(0, 100);
            this.pnlSettingsBody.Name = "pnlSettingsBody";
            this.pnlSettingsBody.Size = new System.Drawing.Size(714, 416);
            this.pnlSettingsBody.TabIndex = 1;
            // 
            // pnlOptions
            // 
            this.pnlOptions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlOptions.BackColor = System.Drawing.Color.White;
            this.pnlOptions.Controls.Add(this.chkNotificationSound);
            this.pnlOptions.Controls.Add(this.chkNotifications);
            this.pnlOptions.Controls.Add(this.lblOptionsTitle);
            this.pnlOptions.Location = new System.Drawing.Point(20, 266);
            this.pnlOptions.Name = "pnlOptions";
            this.pnlOptions.Size = new System.Drawing.Size(670, 200);
            this.pnlOptions.TabIndex = 2;
            // 
            // chkNotificationSound
            // 
            this.chkNotificationSound.AutoSize = true;
            this.chkNotificationSound.Enabled = false;
            this.chkNotificationSound.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.chkNotificationSound.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(24)))), ((int)(((byte)(90)))));
            this.chkNotificationSound.Location = new System.Drawing.Point(24, 108);
            this.chkNotificationSound.Name = "chkNotificationSound";
            this.chkNotificationSound.Size = new System.Drawing.Size(238, 29);
            this.chkNotificationSound.TabIndex = 2;
            this.chkNotificationSound.Text = "Bật âm thanh thông báo";
            this.chkNotificationSound.UseVisualStyleBackColor = true;
            // 
            // chkNotifications
            // 
            this.chkNotifications.AutoSize = true;
            this.chkNotifications.Enabled = false;
            this.chkNotifications.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.chkNotifications.Location = new System.Drawing.Point(24, 65);
            this.chkNotifications.Name = "chkNotifications";
            this.chkNotifications.Size = new System.Drawing.Size(228, 29);
            this.chkNotifications.TabIndex = 1;
            this.chkNotifications.Text = "Bật thông báo tin nhắn";
            this.chkNotifications.UseVisualStyleBackColor = true;
            // 
            // lblOptionsTitle
            // 
            this.lblOptionsTitle.AutoSize = true;
            this.lblOptionsTitle.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this.lblOptionsTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(24)))), ((int)(((byte)(90)))));
            this.lblOptionsTitle.Location = new System.Drawing.Point(20, 15);
            this.lblOptionsTitle.Name = "lblOptionsTitle";
            this.lblOptionsTitle.Size = new System.Drawing.Size(241, 35);
            this.lblOptionsTitle.TabIndex = 0;
            this.lblOptionsTitle.Text = "Tùy chọn ứng dụng";
            this.lblOptionsTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tblSettingsCards
            // 
            this.tblSettingsCards.ColumnCount = 2;
            this.tblSettingsCards.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblSettingsCards.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblSettingsCards.Controls.Add(this.pnlAccountInfo, 0, 0);
            this.tblSettingsCards.Controls.Add(this.pnlConnectionInfo, 1, 0);
            this.tblSettingsCards.Dock = System.Windows.Forms.DockStyle.Top;
            this.tblSettingsCards.Location = new System.Drawing.Point(0, 0);
            this.tblSettingsCards.Name = "tblSettingsCards";
            this.tblSettingsCards.Padding = new System.Windows.Forms.Padding(10);
            this.tblSettingsCards.RowCount = 1;
            this.tblSettingsCards.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblSettingsCards.Size = new System.Drawing.Size(690, 260);
            this.tblSettingsCards.TabIndex = 1;
            // 
            // pnlAccountInfo
            // 
            this.pnlAccountInfo.BackColor = System.Drawing.Color.White;
            this.pnlAccountInfo.Controls.Add(this.lblAccountSectionTitle);
            this.pnlAccountInfo.Controls.Add(this.lblAccountUsernameCaption);
            this.pnlAccountInfo.Controls.Add(this.lblAccountUsernameValue);
            this.pnlAccountInfo.Controls.Add(this.lblAccountStatusCaption);
            this.pnlAccountInfo.Controls.Add(this.lblAccountStatusValue);
            this.pnlAccountInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlAccountInfo.Location = new System.Drawing.Point(20, 20);
            this.pnlAccountInfo.Margin = new System.Windows.Forms.Padding(10);
            this.pnlAccountInfo.Name = "pnlAccountInfo";
            this.pnlAccountInfo.Padding = new System.Windows.Forms.Padding(12);
            this.pnlAccountInfo.Size = new System.Drawing.Size(315, 220);
            this.pnlAccountInfo.TabIndex = 1;
            // 
            // lblAccountSectionTitle
            // 
            this.lblAccountSectionTitle.AutoSize = true;
            this.lblAccountSectionTitle.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this.lblAccountSectionTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(24)))), ((int)(((byte)(90)))));
            this.lblAccountSectionTitle.Location = new System.Drawing.Point(20, 12);
            this.lblAccountSectionTitle.Name = "lblAccountSectionTitle";
            this.lblAccountSectionTitle.Size = new System.Drawing.Size(245, 35);
            this.lblAccountSectionTitle.TabIndex = 0;
            this.lblAccountSectionTitle.Text = "Thông tin tài khoản";
            // 
            // lblAccountUsernameCaption
            // 
            this.lblAccountUsernameCaption.AutoSize = true;
            this.lblAccountUsernameCaption.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblAccountUsernameCaption.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(36)))), ((int)(((byte)(140)))));
            this.lblAccountUsernameCaption.Location = new System.Drawing.Point(24, 65);
            this.lblAccountUsernameCaption.Name = "lblAccountUsernameCaption";
            this.lblAccountUsernameCaption.Size = new System.Drawing.Size(149, 25);
            this.lblAccountUsernameCaption.TabIndex = 1;
            this.lblAccountUsernameCaption.Text = "Tên đăng nhập:";
            // 
            // lblAccountUsernameValue
            // 
            this.lblAccountUsernameValue.AutoSize = true;
            this.lblAccountUsernameValue.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblAccountUsernameValue.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(36)))), ((int)(((byte)(140)))));
            this.lblAccountUsernameValue.Location = new System.Drawing.Point(165, 65);
            this.lblAccountUsernameValue.Name = "lblAccountUsernameValue";
            this.lblAccountUsernameValue.Size = new System.Drawing.Size(159, 25);
            this.lblAccountUsernameValue.TabIndex = 2;
            this.lblAccountUsernameValue.Text = "Chưa đăng nhập";
            // 
            // lblAccountStatusCaption
            // 
            this.lblAccountStatusCaption.AutoSize = true;
            this.lblAccountStatusCaption.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblAccountStatusCaption.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(36)))), ((int)(((byte)(140)))));
            this.lblAccountStatusCaption.Location = new System.Drawing.Point(24, 110);
            this.lblAccountStatusCaption.Name = "lblAccountStatusCaption";
            this.lblAccountStatusCaption.Size = new System.Drawing.Size(107, 25);
            this.lblAccountStatusCaption.TabIndex = 3;
            this.lblAccountStatusCaption.Text = "Trạng thái:";
            // 
            // lblAccountStatusValue
            // 
            this.lblAccountStatusValue.AutoSize = true;
            this.lblAccountStatusValue.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblAccountStatusValue.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(36)))), ((int)(((byte)(140)))));
            this.lblAccountStatusValue.Location = new System.Drawing.Point(170, 110);
            this.lblAccountStatusValue.Name = "lblAccountStatusValue";
            this.lblAccountStatusValue.Size = new System.Drawing.Size(122, 25);
            this.lblAccountStatusValue.TabIndex = 4;
            this.lblAccountStatusValue.Text = "Ngoại tuyến";
            // 
            // pnlConnectionInfo
            // 
            this.pnlConnectionInfo.BackColor = System.Drawing.Color.White;
            this.pnlConnectionInfo.Controls.Add(this.lblConnectionSectionTitle);
            this.pnlConnectionInfo.Controls.Add(this.lblServerIPCaption);
            this.pnlConnectionInfo.Controls.Add(this.lblServerIPValue);
            this.pnlConnectionInfo.Controls.Add(this.lblPortCaption);
            this.pnlConnectionInfo.Controls.Add(this.lblPortValue);
            this.pnlConnectionInfo.Controls.Add(this.lblConnectionStatusCaption);
            this.pnlConnectionInfo.Controls.Add(this.lblConnectionStatusValue);
            this.pnlConnectionInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlConnectionInfo.Location = new System.Drawing.Point(355, 20);
            this.pnlConnectionInfo.Margin = new System.Windows.Forms.Padding(10);
            this.pnlConnectionInfo.Name = "pnlConnectionInfo";
            this.pnlConnectionInfo.Size = new System.Drawing.Size(315, 220);
            this.pnlConnectionInfo.TabIndex = 2;
            // 
            // lblConnectionSectionTitle
            // 
            this.lblConnectionSectionTitle.AutoSize = true;
            this.lblConnectionSectionTitle.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this.lblConnectionSectionTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(24)))), ((int)(((byte)(90)))));
            this.lblConnectionSectionTitle.Location = new System.Drawing.Point(20, 12);
            this.lblConnectionSectionTitle.Name = "lblConnectionSectionTitle";
            this.lblConnectionSectionTitle.Size = new System.Drawing.Size(218, 35);
            this.lblConnectionSectionTitle.TabIndex = 0;
            this.lblConnectionSectionTitle.Text = "Thông tin kết nối";
            // 
            // lblServerIPCaption
            // 
            this.lblServerIPCaption.AutoSize = true;
            this.lblServerIPCaption.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblServerIPCaption.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(36)))), ((int)(((byte)(140)))));
            this.lblServerIPCaption.Location = new System.Drawing.Point(20, 60);
            this.lblServerIPCaption.Name = "lblServerIPCaption";
            this.lblServerIPCaption.Size = new System.Drawing.Size(103, 25);
            this.lblServerIPCaption.TabIndex = 1;
            this.lblServerIPCaption.Text = "Server  IP:";
            // 
            // lblServerIPValue
            // 
            this.lblServerIPValue.AutoSize = true;
            this.lblServerIPValue.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblServerIPValue.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(36)))), ((int)(((byte)(140)))));
            this.lblServerIPValue.Location = new System.Drawing.Point(170, 60);
            this.lblServerIPValue.Name = "lblServerIPValue";
            this.lblServerIPValue.Size = new System.Drawing.Size(20, 25);
            this.lblServerIPValue.TabIndex = 2;
            this.lblServerIPValue.Text = "-";
            // 
            // lblPortCaption
            // 
            this.lblPortCaption.AutoSize = true;
            this.lblPortCaption.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblPortCaption.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(36)))), ((int)(((byte)(140)))));
            this.lblPortCaption.Location = new System.Drawing.Point(20, 100);
            this.lblPortCaption.Name = "lblPortCaption";
            this.lblPortCaption.Size = new System.Drawing.Size(56, 25);
            this.lblPortCaption.TabIndex = 3;
            this.lblPortCaption.Text = "Port:";
            // 
            // lblPortValue
            // 
            this.lblPortValue.AutoSize = true;
            this.lblPortValue.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblPortValue.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(36)))), ((int)(((byte)(140)))));
            this.lblPortValue.Location = new System.Drawing.Point(170, 100);
            this.lblPortValue.Name = "lblPortValue";
            this.lblPortValue.Size = new System.Drawing.Size(20, 25);
            this.lblPortValue.TabIndex = 4;
            this.lblPortValue.Text = "-";
            // 
            // lblConnectionStatusCaption
            // 
            this.lblConnectionStatusCaption.AutoSize = true;
            this.lblConnectionStatusCaption.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblConnectionStatusCaption.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(36)))), ((int)(((byte)(140)))));
            this.lblConnectionStatusCaption.Location = new System.Drawing.Point(20, 140);
            this.lblConnectionStatusCaption.Name = "lblConnectionStatusCaption";
            this.lblConnectionStatusCaption.Size = new System.Drawing.Size(107, 25);
            this.lblConnectionStatusCaption.TabIndex = 5;
            this.lblConnectionStatusCaption.Text = "Trạng thái:";
            // 
            // lblConnectionStatusValue
            // 
            this.lblConnectionStatusValue.AutoSize = true;
            this.lblConnectionStatusValue.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblConnectionStatusValue.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(74)))), ((int)(((byte)(36)))), ((int)(((byte)(140)))));
            this.lblConnectionStatusValue.Location = new System.Drawing.Point(170, 140);
            this.lblConnectionStatusValue.Name = "lblConnectionStatusValue";
            this.lblConnectionStatusValue.Size = new System.Drawing.Size(125, 25);
            this.lblConnectionStatusValue.TabIndex = 6;
            this.lblConnectionStatusValue.Text = "Chưa kết nối";
            // 
            // pnlSettingsHeader
            // 
            this.pnlSettingsHeader.Controls.Add(this.btnBackToChat);
            this.pnlSettingsHeader.Controls.Add(this.lblSettingsSubtitle);
            this.pnlSettingsHeader.Controls.Add(this.lblSettingsTitle);
            this.pnlSettingsHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlSettingsHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlSettingsHeader.Name = "pnlSettingsHeader";
            this.pnlSettingsHeader.Size = new System.Drawing.Size(714, 100);
            this.pnlSettingsHeader.TabIndex = 0;
            // 
            // btnBackToChat
            // 
            this.btnBackToChat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBackToChat.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(36)))), ((int)(((byte)(140)))));
            this.btnBackToChat.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBackToChat.FlatAppearance.BorderSize = 0;
            this.btnBackToChat.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBackToChat.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnBackToChat.ForeColor = System.Drawing.Color.White;
            this.btnBackToChat.Location = new System.Drawing.Point(546, 29);
            this.btnBackToChat.Name = "btnBackToChat";
            this.btnBackToChat.Size = new System.Drawing.Size(133, 52);
            this.btnBackToChat.TabIndex = 1;
            this.btnBackToChat.Text = "←   Quay lại";
            this.btnBackToChat.UseVisualStyleBackColor = false;
            this.btnBackToChat.Click += new System.EventHandler(this.btnBackToChat_Click);
            // 
            // lblSettingsSubtitle
            // 
            this.lblSettingsSubtitle.AutoSize = true;
            this.lblSettingsSubtitle.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblSettingsSubtitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(110)))), ((int)(((byte)(105)))), ((int)(((byte)(140)))));
            this.lblSettingsSubtitle.Location = new System.Drawing.Point(23, 58);
            this.lblSettingsSubtitle.Name = "lblSettingsSubtitle";
            this.lblSettingsSubtitle.Size = new System.Drawing.Size(471, 25);
            this.lblSettingsSubtitle.TabIndex = 1;
            this.lblSettingsSubtitle.Text = "Quản lý thông tin cá nhân, kết nối và tùy chọn của bạn";
            // 
            // lblSettingsTitle
            // 
            this.lblSettingsTitle.AutoSize = true;
            this.lblSettingsTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblSettingsTitle.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold);
            this.lblSettingsTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(24)))), ((int)(((byte)(90)))));
            this.lblSettingsTitle.Location = new System.Drawing.Point(19, 23);
            this.lblSettingsTitle.Name = "lblSettingsTitle";
            this.lblSettingsTitle.Size = new System.Drawing.Size(322, 51);
            this.lblSettingsTitle.TabIndex = 1;
            this.lblSettingsTitle.Text = "Cài đặt tài khoản";
            // 
            // pnlChatFlow
            // 
            this.pnlChatFlow.AutoScroll = true;
            this.pnlChatFlow.BackColor = System.Drawing.Color.White;
            this.pnlChatFlow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlChatFlow.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.pnlChatFlow.Location = new System.Drawing.Point(0, 70);
            this.pnlChatFlow.Name = "pnlChatFlow";
            this.pnlChatFlow.Padding = new System.Windows.Forms.Padding(10, 6, 10, 6);
            this.pnlChatFlow.Size = new System.Drawing.Size(714, 516);
            this.pnlChatFlow.TabIndex = 3;
            this.pnlChatFlow.WrapContents = false;
            this.pnlChatFlow.Resize += new System.EventHandler(this.pnlChatFlow_Resize);
            // 
            // pnlReplyPreview
            // 
            this.pnlReplyPreview.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            this.pnlReplyPreview.Controls.Add(this.btnCancelReply);
            this.pnlReplyPreview.Controls.Add(this.lblReplyPreviewText);
            this.pnlReplyPreview.Controls.Add(this.pnlReplyBar);
            this.pnlReplyPreview.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlReplyPreview.Location = new System.Drawing.Point(0, 586);
            this.pnlReplyPreview.Name = "pnlReplyPreview";
            this.pnlReplyPreview.Size = new System.Drawing.Size(714, 42);
            this.pnlReplyPreview.TabIndex = 2;
            this.pnlReplyPreview.Visible = false;
            // 
            // btnCancelReply
            // 
            this.btnCancelReply.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancelReply.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnCancelReply.FlatAppearance.BorderSize = 0;
            this.btnCancelReply.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelReply.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnCancelReply.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.btnCancelReply.Location = new System.Drawing.Point(674, 0);
            this.btnCancelReply.Name = "btnCancelReply";
            this.btnCancelReply.Size = new System.Drawing.Size(40, 42);
            this.btnCancelReply.TabIndex = 2;
            this.btnCancelReply.Text = "✕";
            this.btnCancelReply.UseVisualStyleBackColor = true;
            this.btnCancelReply.Click += new System.EventHandler(this.btnCancelReply_Click);
            // 
            // lblReplyPreviewText
            // 
            this.lblReplyPreviewText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblReplyPreviewText.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblReplyPreviewText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.lblReplyPreviewText.Location = new System.Drawing.Point(4, 0);
            this.lblReplyPreviewText.Name = "lblReplyPreviewText";
            this.lblReplyPreviewText.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.lblReplyPreviewText.Size = new System.Drawing.Size(710, 42);
            this.lblReplyPreviewText.TabIndex = 1;
            this.lblReplyPreviewText.Text = "Đang trả lời: ...";
            this.lblReplyPreviewText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnlReplyBar
            // 
            this.pnlReplyBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(132)))), ((int)(((byte)(255)))));
            this.pnlReplyBar.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlReplyBar.Location = new System.Drawing.Point(0, 0);
            this.pnlReplyBar.Name = "pnlReplyBar";
            this.pnlReplyBar.Size = new System.Drawing.Size(4, 42);
            this.pnlReplyBar.TabIndex = 0;
            // 
            // pnlInput
            // 
            this.pnlInput.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(247)))), ((int)(((byte)(252)))));
            this.pnlInput.Controls.Add(this.btnSend);
            this.pnlInput.Controls.Add(this.txtMessage);
            this.pnlInput.Controls.Add(this.btnEmoji);
            this.pnlInput.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlInput.Location = new System.Drawing.Point(0, 628);
            this.pnlInput.Name = "pnlInput";
            this.pnlInput.Padding = new System.Windows.Forms.Padding(10);
            this.pnlInput.Size = new System.Drawing.Size(714, 72);
            this.pnlInput.TabIndex = 1;
            // 
            // btnSend
            // 
            this.btnSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSend.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(36)))), ((int)(((byte)(145)))));
            this.btnSend.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSend.FlatAppearance.BorderSize = 0;
            this.btnSend.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSend.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnSend.ForeColor = System.Drawing.Color.White;
            this.btnSend.Location = new System.Drawing.Point(622, 11);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(80, 42);
            this.btnSend.TabIndex = 2;
            this.btnSend.Text = "Gửi";
            this.btnSend.UseVisualStyleBackColor = false;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // txtMessage
            // 
            this.txtMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMessage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtMessage.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtMessage.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(24)))), ((int)(((byte)(90)))));
            this.txtMessage.Location = new System.Drawing.Point(62, 11);
            this.txtMessage.Multiline = true;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtMessage.Size = new System.Drawing.Size(552, 42);
            this.txtMessage.TabIndex = 1;
            this.txtMessage.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtMessage_KeyDown);
            // 
            // btnEmoji
            // 
            this.btnEmoji.BackColor = System.Drawing.Color.Transparent;
            this.btnEmoji.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEmoji.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(205)))), ((int)(((byte)(215)))));
            this.btnEmoji.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEmoji.Font = new System.Drawing.Font("Segoe UI Emoji", 14F);
            this.btnEmoji.Location = new System.Drawing.Point(12, 11);
            this.btnEmoji.Name = "btnEmoji";
            this.btnEmoji.Size = new System.Drawing.Size(44, 42);
            this.btnEmoji.TabIndex = 0;
            this.btnEmoji.Text = "😊";
            this.btnEmoji.UseVisualStyleBackColor = false;
            this.btnEmoji.Click += new System.EventHandler(this.btnEmoji_Click);
            // 
            // pnlChatHeader
            // 
            this.pnlChatHeader.BackColor = System.Drawing.Color.White;
            this.pnlChatHeader.Controls.Add(this.pnlChatActions);
            this.pnlChatHeader.Controls.Add(this.lblChatSub);
            this.pnlChatHeader.Controls.Add(this.lblCurrentChat);
            this.pnlChatHeader.Controls.Add(this.picChatHeaderAvatar);
            this.pnlChatHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlChatHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlChatHeader.Name = "pnlChatHeader";
            this.pnlChatHeader.Size = new System.Drawing.Size(714, 70);
            this.pnlChatHeader.TabIndex = 4;
            this.pnlChatHeader.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlChatHeader_Paint);
            // 
            // pnlChatActions
            // 
            this.pnlChatActions.BackColor = System.Drawing.Color.Transparent;
            this.pnlChatActions.Controls.Add(this.btnHeaderMore);
            this.pnlChatActions.Controls.Add(this.btnHeaderVideo);
            this.pnlChatActions.Controls.Add(this.btnHeaderCall);
            this.pnlChatActions.Controls.Add(this.btnHeaderSearch);
            this.pnlChatActions.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlChatActions.ForeColor = System.Drawing.SystemColors.ControlText;
            this.pnlChatActions.Location = new System.Drawing.Point(514, 0);
            this.pnlChatActions.Name = "pnlChatActions";
            this.pnlChatActions.Size = new System.Drawing.Size(200, 70);
            this.pnlChatActions.TabIndex = 3;
            // 
            // btnHeaderMore
            // 
            this.btnHeaderMore.Enabled = false;
            this.btnHeaderMore.FlatAppearance.BorderSize = 0;
            this.btnHeaderMore.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHeaderMore.Font = new System.Drawing.Font("Segoe UI", 15F);
            this.btnHeaderMore.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(36)))), ((int)(((byte)(140)))));
            this.btnHeaderMore.Location = new System.Drawing.Point(150, 17);
            this.btnHeaderMore.Name = "btnHeaderMore";
            this.btnHeaderMore.Size = new System.Drawing.Size(36, 42);
            this.btnHeaderMore.TabIndex = 3;
            this.btnHeaderMore.Text = "⋯";
            this.btnHeaderMore.UseVisualStyleBackColor = true;
            // 
            // btnHeaderVideo
            // 
            this.btnHeaderVideo.Enabled = false;
            this.btnHeaderVideo.FlatAppearance.BorderSize = 0;
            this.btnHeaderVideo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHeaderVideo.Font = new System.Drawing.Font("Segoe UI", 15F);
            this.btnHeaderVideo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(36)))), ((int)(((byte)(140)))));
            this.btnHeaderVideo.Location = new System.Drawing.Point(108, 20);
            this.btnHeaderVideo.Name = "btnHeaderVideo";
            this.btnHeaderVideo.Size = new System.Drawing.Size(36, 42);
            this.btnHeaderVideo.TabIndex = 2;
            this.btnHeaderVideo.Text = "🎥";
            this.btnHeaderVideo.UseVisualStyleBackColor = true;
            // 
            // btnHeaderCall
            // 
            this.btnHeaderCall.Enabled = false;
            this.btnHeaderCall.FlatAppearance.BorderSize = 0;
            this.btnHeaderCall.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHeaderCall.Font = new System.Drawing.Font("Segoe UI Symbol", 15F);
            this.btnHeaderCall.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(36)))), ((int)(((byte)(140)))));
            this.btnHeaderCall.Location = new System.Drawing.Point(66, 20);
            this.btnHeaderCall.Name = "btnHeaderCall";
            this.btnHeaderCall.Size = new System.Drawing.Size(36, 42);
            this.btnHeaderCall.TabIndex = 1;
            this.btnHeaderCall.Text = "☎";
            this.btnHeaderCall.UseVisualStyleBackColor = true;
            // 
            // btnHeaderSearch
            // 
            this.btnHeaderSearch.Enabled = false;
            this.btnHeaderSearch.FlatAppearance.BorderSize = 0;
            this.btnHeaderSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHeaderSearch.Font = new System.Drawing.Font("Segoe UI Symbol", 15F);
            this.btnHeaderSearch.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(36)))), ((int)(((byte)(140)))));
            this.btnHeaderSearch.Location = new System.Drawing.Point(24, 20);
            this.btnHeaderSearch.Name = "btnHeaderSearch";
            this.btnHeaderSearch.Size = new System.Drawing.Size(36, 42);
            this.btnHeaderSearch.TabIndex = 0;
            this.btnHeaderSearch.Text = "⌕";
            this.btnHeaderSearch.UseVisualStyleBackColor = true;
            // 
            // lblChatSub
            // 
            this.lblChatSub.AutoSize = true;
            this.lblChatSub.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblChatSub.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(110)))), ((int)(((byte)(105)))), ((int)(((byte)(140)))));
            this.lblChatSub.Location = new System.Drawing.Point(78, 39);
            this.lblChatSub.Name = "lblChatSub";
            this.lblChatSub.Size = new System.Drawing.Size(429, 23);
            this.lblChatSub.TabIndex = 2;
            this.lblChatSub.Text = "Kênh công khai - tất cả thành viên có thể đọc tin nhắn";
            // 
            // lblCurrentChat
            // 
            this.lblCurrentChat.AutoSize = true;
            this.lblCurrentChat.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this.lblCurrentChat.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(24)))), ((int)(((byte)(90)))));
            this.lblCurrentChat.Location = new System.Drawing.Point(78, 12);
            this.lblCurrentChat.Name = "lblCurrentChat";
            this.lblCurrentChat.Size = new System.Drawing.Size(337, 35);
            this.lblCurrentChat.TabIndex = 1;
            this.lblCurrentChat.Text = "Đang trò chuyện với: Tất cả";
            // 
            // picChatHeaderAvatar
            // 
            this.picChatHeaderAvatar.Location = new System.Drawing.Point(16, 12);
            this.picChatHeaderAvatar.Name = "picChatHeaderAvatar";
            this.picChatHeaderAvatar.Size = new System.Drawing.Size(46, 46);
            this.picChatHeaderAvatar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picChatHeaderAvatar.TabIndex = 0;
            this.picChatHeaderAvatar.TabStop = false;
            // 
            // ClientForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 23F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1100, 700);
            this.Controls.Add(this.splitContainerMain);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimumSize = new System.Drawing.Size(900, 600);
            this.Name = "ClientForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "WeTALK - Trò chuyện trực tuyến";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ClientForm_FormClosing);
            this.splitContainerMain.Panel1.ResumeLayout(false);
            this.splitContainerMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).EndInit();
            this.splitContainerMain.ResumeLayout(false);
            this.pnlUsersHeader.ResumeLayout(false);
            this.pnlNavigation.ResumeLayout(false);
            this.pnlSearch.ResumeLayout(false);
            this.pnlSearch.PerformLayout();
            this.pnlBrand.ResumeLayout(false);
            this.pnlBrand.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picBrandLogo)).EndInit();
            this.pnlUserProfile.ResumeLayout(false);
            this.pnlUserProfile.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picOwnAvatar)).EndInit();
            this.pnlSettingsView.ResumeLayout(false);
            this.pnlSettingsView.PerformLayout();
            this.pnlSettingsBody.ResumeLayout(false);
            this.pnlOptions.ResumeLayout(false);
            this.pnlOptions.PerformLayout();
            this.tblSettingsCards.ResumeLayout(false);
            this.pnlAccountInfo.ResumeLayout(false);
            this.pnlAccountInfo.PerformLayout();
            this.pnlConnectionInfo.ResumeLayout(false);
            this.pnlConnectionInfo.PerformLayout();
            this.pnlSettingsHeader.ResumeLayout(false);
            this.pnlSettingsHeader.PerformLayout();
            this.pnlReplyPreview.ResumeLayout(false);
            this.pnlInput.ResumeLayout(false);
            this.pnlInput.PerformLayout();
            this.pnlChatHeader.ResumeLayout(false);
            this.pnlChatHeader.PerformLayout();
            this.pnlChatActions.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picChatHeaderAvatar)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainerMain;
        private System.Windows.Forms.Panel pnlUserProfile;
        private System.Windows.Forms.PictureBox picOwnAvatar;
        private System.Windows.Forms.Label lblOwnUsername;
        private System.Windows.Forms.Label lblOwnStatusDot;
        private System.Windows.Forms.Label lblOwnStatus;
        private System.Windows.Forms.Panel pnlUsersHeader;
        private System.Windows.Forms.Label lblUsersHeader;
        private System.Windows.Forms.ListBox lstUsers;
        private System.Windows.Forms.Panel pnlChatHeader;
        private System.Windows.Forms.PictureBox picChatHeaderAvatar;
        private System.Windows.Forms.Label lblCurrentChat;
        private System.Windows.Forms.Label lblChatSub;
        private System.Windows.Forms.FlowLayoutPanel pnlChatFlow;
        private System.Windows.Forms.Panel pnlReplyPreview;
        private System.Windows.Forms.Panel pnlReplyBar;
        private System.Windows.Forms.Label lblReplyPreviewText;
        private System.Windows.Forms.Button btnCancelReply;
        private System.Windows.Forms.Panel pnlInput;
        private System.Windows.Forms.Button btnEmoji;
        private System.Windows.Forms.TextBox txtMessage;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.Panel pnlBrand;
        private System.Windows.Forms.PictureBox picBrandLogo;
        private System.Windows.Forms.Label lblBrandName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel pnlSearch;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Panel pnlNavigation;
        private System.Windows.Forms.Button btnChats;
        private System.Windows.Forms.Button btnNewGroup;
        private System.Windows.Forms.Button btnGroups;
        private System.Windows.Forms.Label lblSearchIcon;
        private System.Windows.Forms.Button btnSettings;
        private System.Windows.Forms.Panel pnlSettingsView;
        private System.Windows.Forms.Label lblSettingsTitle;
        private System.Windows.Forms.Panel pnlSettingsHeader;
        private System.Windows.Forms.Label lblSettingsSubtitle;
        private System.Windows.Forms.Button btnBackToChat;
        private System.Windows.Forms.Panel pnlSettingsBody;
        private System.Windows.Forms.TableLayoutPanel tblSettingsCards;
        private System.Windows.Forms.Panel pnlConnectionInfo;
        private System.Windows.Forms.Label lblConnectionSectionTitle;
        private System.Windows.Forms.Label lblPortCaption;
        private System.Windows.Forms.Label lblServerIPValue;
        private System.Windows.Forms.Label lblServerIPCaption;
        private System.Windows.Forms.Label lblConnectionStatusValue;
        private System.Windows.Forms.Label lblConnectionStatusCaption;
        private System.Windows.Forms.Label lblPortValue;
        private System.Windows.Forms.Panel pnlAccountInfo;
        private System.Windows.Forms.Label lblAccountSectionTitle;
        private System.Windows.Forms.Label lblAccountUsernameCaption;
        private System.Windows.Forms.Label lblAccountUsernameValue;
        private System.Windows.Forms.Label lblAccountStatusCaption;
        private System.Windows.Forms.Label lblAccountStatusValue;
        private System.Windows.Forms.Panel pnlOptions;
        private System.Windows.Forms.CheckBox chkNotificationSound;
        private System.Windows.Forms.CheckBox chkNotifications;
        private System.Windows.Forms.Label lblOptionsTitle;
        private System.Windows.Forms.Panel pnlSettingsFooter;
        private System.Windows.Forms.Panel pnlChatActions;
        private System.Windows.Forms.Button btnHeaderMore;
        private System.Windows.Forms.Button btnHeaderVideo;
        private System.Windows.Forms.Button btnHeaderCall;
        private System.Windows.Forms.Button btnHeaderSearch;
    }
}