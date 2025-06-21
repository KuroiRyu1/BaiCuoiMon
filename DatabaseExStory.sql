-- Tạo cơ sở dữ liệu
CREATE DATABASE Ex_web_story;
GO

USE Ex_web_story;
GO

-- Bảng tác giả
CREATE TABLE tbl_author (
    _id INT IDENTITY(1,1) PRIMARY KEY,
    _name NVARCHAR(200),
    _information NVARCHAR(500),
    _image VARCHAR(50)
);
GO

-- Bảng danh mục
CREATE TABLE tbl_category (
    _id INT IDENTITY(1,1) PRIMARY KEY,
    _name NVARCHAR(100),
    _active INT
);
GO

-- Bảng trạng thái
CREATE TABLE tbl_status (
    _id INT IDENTITY(1,1) PRIMARY KEY,
    _title NVARCHAR(100),
    _active INT
);
GO

-- Bảng loại truyện
CREATE TABLE tbl_story_type (
    _id INT IDENTITY(1,1) PRIMARY KEY,
    _title NVARCHAR(100),
    _active INT
);
GO

-- Bảng người dùng
CREATE TABLE tbl_user (
    _id BIGINT IDENTITY(1,1) PRIMARY KEY,
    _username NVARCHAR(50),
    _score INT,
    _active INT,
    _password NVARCHAR(255),
    _fullname NVARCHAR(100),
    _token NVARCHAR(20),
    _role INT,
    _email NVARCHAR(50)
);
GO

-- Bảng truyện
CREATE TABLE tbl_story (
    _id INT IDENTITY(1,1) PRIMARY KEY,
    _title NVARCHAR(200),
    _chapter_number INT,
    _introduction NVARCHAR(500),
    _image VARCHAR(50),
    _like_number INT,
    _follow_number INT,
    _view_number DECIMAL(18,0),
    _author_id INT,
    _status_id INT,
    _category_id INT,
    _story_type_id INT,
    _active INT DEFAULT 1,
    FOREIGN KEY (_author_id) REFERENCES tbl_author(_id),
    FOREIGN KEY (_status_id) REFERENCES tbl_status(_id),
    FOREIGN KEY (_category_id) REFERENCES tbl_category(_id),
    FOREIGN KEY (_story_type_id) REFERENCES tbl_story_type(_id)
);
GO

-- Bảng chương
CREATE TABLE tbl_chapter (
    _id INT IDENTITY(1,1) PRIMARY KEY,
    _title NVARCHAR(200),
    _content NTEXT,
    _day_create DATE,
    _story_id INT,
    _chapter_index INT,
    _active INT DEFAULT 1,
    FOREIGN KEY (_story_id) REFERENCES tbl_story(_id)
);
GO

-- Bảng hình ảnh chương
CREATE TABLE tbl_chapter_image (
    _id BIGINT IDENTITY(1,1) PRIMARY KEY,
    _image VARCHAR(50),
    _index INT,
    _chapter_id INT,
    FOREIGN KEY (_chapter_id) REFERENCES tbl_chapter(_id),
    CONSTRAINT uq_chapter_image UNIQUE (_chapter_id, _index)
);
GO

-- Bảng bình luận chương
CREATE TABLE tbl_chapter_comment (
    _id BIGINT IDENTITY(1,1) PRIMARY KEY,
    _content NVARCHAR(500),
    _active INT,
    _chapter_id INT,
    _user_id BIGINT,
    FOREIGN KEY (_chapter_id) REFERENCES tbl_chapter(_id),
    FOREIGN KEY (_user_id) REFERENCES tbl_user(_id)
);
GO

-- Bảng bình luận truyện
CREATE TABLE tbl_story_comment (
    _id BIGINT IDENTITY(1,1) PRIMARY KEY,
    _content NVARCHAR(500),
    _active INT,
    _story_id INT,
    _user_id BIGINT,
    FOREIGN KEY (_story_id) REFERENCES tbl_story(_id),
    FOREIGN KEY (_user_id) REFERENCES tbl_user(_id)
);
GO

-- Bảng follow truyện
CREATE TABLE tbl_story_follow (
    _id BIGINT IDENTITY(1,1) PRIMARY KEY,
    _user_id BIGINT,
    _story_id INT,
    _day_create DATE,
    _status INT, -- 1 = follow, 0 = unfollow
    FOREIGN KEY (_story_id) REFERENCES tbl_story(_id),
    FOREIGN KEY (_user_id) REFERENCES tbl_user(_id)
);
GO

-- Bảng like truyện
CREATE TABLE tbl_story_like (
    _id BIGINT IDENTITY(1,1) PRIMARY KEY,
    _user_id BIGINT,
    _story_id INT,
    _day_create DATE,
    _status INT, -- 1 = like, 0 = unlike
    FOREIGN KEY (_story_id) REFERENCES tbl_story(_id),
    FOREIGN KEY (_user_id) REFERENCES tbl_user(_id)
);
GO

-- Thêm dữ liệu mẫu

-- Thêm dữ liệu vào tbl_author
INSERT INTO tbl_author (_name, _information, _image) VALUES
(N'Nguyễn Nhật Ánh', N'Tác giả nổi tiếng với các truyện tuổi học trò.', N'images/authors/nguyen_nhat_anh.jpg'),
(N'Trần Văn A', N'Tác giả trẻ, chuyên truyện phiêu lưu.', N'images/authors/tran_van_a.jpg'),
(N'Lê Thị B', N'Tác giả nữ với phong cách lãng mạn.', N'images/authors/le_thi_b.jpg');
GO

-- Thêm dữ liệu vào tbl_category
INSERT INTO tbl_category (_name, _active) VALUES
(N'cate1', 1),
(N'cate2', 1),
(N'cate3', 1),
(N'cate4', 1),
(N'cate5', 1),
(N'cate6', 1);
GO

-- Thêm dữ liệu vào tbl_status
INSERT INTO tbl_status (_title, _active) VALUES
(N'Đang tiến hành', 1),
(N'Đã hoàn thành', 1),
(N'Tạm ngưng', 0);
GO

-- Thêm dữ liệu vào tbl_story_type
INSERT INTO tbl_story_type (_title, _active) VALUES
(N'Truyện ngắn', 1),
(N'Truyện dài', 1),
(N'Truyện tranh', 1);
GO

-- Thêm dữ liệu vào tbl_user
INSERT INTO tbl_user (_username, _score, _active, _password, _fullname, _token, _role, _email) VALUES
(N'admin3', NULL, 1, N'$2a$11$GUlot4XNH703lOpriJeVLe3xQ5J7rb1aMeo3H6bHsnG3IRsTxMV5u', NULL, NULL, 0, N'admin3@gmail.com'),
(N'admin3', NULL, 1, N'$2a$11$ZGrDkodgqheIRs42m4DE/.rn3j5kIKRJqcRaC93pydikIbcDW1xHW', N'Test User', NULL, 1, N'testuser01@example.com'),
(N'admin3', NULL, 1, N'$2a$11$MC7.vOdqBsRL23U5usmcq.xAILupxkFXS0Qvai2CPv4bXeW2jZvPW', N'Test User', NULL, 1, N'testuser01@example.com'),
(N'test0122', NULL, 1, N'$2a$11$i1YdK82zBu2vuqwq2/fJq.qvyGU6nGkh6XrSaDT36vBlfxQd2CyO6', N'Test User', NULL, 1, N'testuser0331@example.com'),
(N'testt0122', NULL, 1, N'$2a$11$WiZFTlpTDIwuAwwMArN67uvxi.76O32/UMtUspLNgmjvsQtTg2rOW', N'Test User', NULL, 1, N'testuser0331@example.com'),
(N'testt0122', NULL, 1, N'$2a$11$HVRm4pzdkUjKHjAdSNAzvuLkBy.ZFgzsPH6CLaKemogS6NBu6wj0i', N'Test User', NULL, 1, N'testuser0331@example.com'),
(N'testt0122', NULL, 1, N'$2a$11$8tTVjULUSj4Y2tEK0uofgOMD3lTPfBS0byHqwYVwNplLhofwejNp6', N'Test User', NULL, 1, N'testuser0331@example.com'),
(N'te01221222', NULL, 1, N'$2a$11$HNs98WEfzyeaPO3lWz6Xyu3cp3JSfBJ/ilqVcm04qRQi0fim0.pWO', N'Test User', NULL, 1, N'testuser0331@example.com'),
(N'te032', NULL, 1, N'$2a$11$J360aLIZ.wtXxmYTnrTj5eCKpcYMHwFApMFJnrf3Q9eYPLD6QzWhC', N'Test User', NULL, 1, N'testuser0331@example.com'),
(N'te0324', NULL, 1, N'$2a$11$ped3wshsoO/a7.mI7.Pzv.XBjpJ14Zp.JsRWLqIa4UeiaRbw9HDgK', N'Test User', NULL, 1, N'testuser0331@example.com'),
(N'adminda', NULL, 1, N'$2a$11$F.hy658LRS6omQaNxTo9ged4vVEHJMWidHenGnUroa6O0OcFv4VXu', NULL, NULL, 0, N'admin@gmail.com'),
(N'adminda1', NULL, 1, N'$2a$11$JaMtnLEj8pC.VsT4Vurf8uVTYho7d58dqJO.qhDia1zhlx27GnB6K', N'Test User', NULL, 1, N'testuser0331@example.com'),
(N'adminda22', NULL, 1, N'$2a$11$xT91W6gLMG45QGPfcM4NUOt9Ay1uVus/8JTQU7slaujt0IxWCS9Im', NULL, NULL, 0, N'admin@localhost.com'),
(N'adminda21', NULL, 1, N'$2a$11$tNrRnDxtVaPhY4Mxpj/h3.Zi9VLSlDv8Rqw4E9frW2JApnp/QMhI6', NULL, NULL, 1, N'testuser0331@example.com'),
(N'admin2d1', NULL, 1, N'$2a$11$AtzMpYh0jIyfMaAJ845v6eUXwk.umY9xKWfuEmLjfYN5RYxNDc.oO', NULL, NULL, 0, N'admin@localhost.com'),
(N'admin2n1d', NULL, 1, N'$2a$11$X7vrvY8bswKiCA9zAY/Qe.gblrRYSHd4GiSvHAnaJw.jVmJ3aYYY6', NULL, NULL, 0, N'admin@gmail.com'),
(N'admin2n1d', NULL, 1, N'$2a$11$0d1lftbMLTkyFWMkyqwUIOSCq/qzd.hQF2aoijzXbUGWLKZytCabu', NULL, NULL, 0, N'admin@gmail.com'),
(N'adminda212', NULL, 1, N'$2a$11$f4hUSGcFvfK6hzEtTR0QvO3m5ekVKLP9Mf8InwH9CscJO3N3XS7mq', NULL, NULL, 0, N'admin@localhost.com');
GO

-- Thêm dữ liệu vào tbl_story
INSERT INTO tbl_story (_title, _chapter_number, _introduction, _image, _like_number, _follow_number, _view_number, _author_id, _status_id, _category_id, _story_type_id, _active) VALUES
(N'Bí Ẩn Rừng Sâu', 12, N'Một hành trình khám phá khu rừng kỳ bí.', N'images/stories/forest.jpg', 150, 45, 2300, 1, 1, 1, 1, 1);
GO