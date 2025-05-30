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
    _username NCHAR(10),
    _score INT,
    _active INT,
    _password NVARCHAR(50),
    _fullname NVARCHAR(100),
    _token NVARCHAR(20),
    _role NVARCHAR(50)
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
    day_create DATE,
    _status INT, --1 = follow, 0 = unfollow
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
    _status INT, --1 = like, 0 = unlike
    FOREIGN KEY (_story_id) REFERENCES tbl_story(_id),
    FOREIGN KEY (_user_id) REFERENCES tbl_user(_id)
);
GO

-- Dữ liệu mẫu
INSERT INTO tbl_category (_name, _active) VALUES
(N'cate1', 1),
(N'cate2', 1),
(N'cate3', 1);
GO
