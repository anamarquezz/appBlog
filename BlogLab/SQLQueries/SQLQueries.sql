

/* CREATING THE  TABLES: */
CREATE TABLE ApplicationUser (
	ApplicationUserId INT NOT NULL IDENTITY(1, 1),
	Username VARCHAR(20) NOT NULL,
	NormalizedUsername VARCHAR(20) NOT NULL,
	Email VARCHAR(30) NOT NULL,
	NormalizedEmail VARCHAR(30) NOT NULL,
	FullName VARCHAR(30) NULL,
	PasswordHash NVARCHAR(MAX) NOT NULL,
	PRIMARY KEY(ApplicationUserId)
)


CREATE TABLE Photo (
	PhotoID INT NOT NULL IDENTITY(1, 1),
	ApplicationUserId INT NOT NULL,
	PublicId VARCHAR(50) NOT NULL,
	ImageUrl VARCHAR(250) NOT NULL,
	[Description] VARCHAR(30) NOT NULL, 
	PublishDate DATETIME NOT NULL DEFAULT GETDATE(),
	UpdateDate DATETIME NOT NULL DEFAULT GETDATE(),
	PRIMARY KEY(PhotoId),
	FOREIGN KEY(ApplicationUserId) REFERENCES ApplicationUser(ApplicationUserId)
)

CREATE TABLE Blog (
	BlogId INT NOT NULL IDENTITY(1, 1),
	ApplicationUserId INT NOT NULL,
	PhotoId INT NULL,
	Title VARCHAR(50) NOT NULL,
	Content VARCHAR(MAX) NOT NULL,
	PublishDate DATETIME NOT NULL DEFAULT GETDATE(),
	UpdateDate DATETIME NOT NULL DEFAULT GETDATE(),
	ActivateInd BIT NOT NULL DEFAULT CONVERT(BIT, 1),
	PRIMARY KEY(blogId),
	FOREIGN KEY(ApplicationUserId) REFERENCES ApplicationUser(ApplicationUserId),
	FOREIGN KEY(PhotoId) REFERENCES Photo(PhotoId)
)

CREATE TABLE BlogComment (
	BlogCommentId INT NOT NULL IDENTITY(1, 1),
	ParentBlogCommentId INT NULL, 
	BlogId INT NOT NULL,
	ApplicationUserId INT NOT NULL,
	Content VARCHAR(300) NOT NULL,
	PublishDate DATETIME NOT NULL DEFAULT GETDATE(),
	UpdateDate DATETIME NOT NULL DEFAULT GETDATE(),
	ActiveInd BIT NOT NULL DEFAULT CONVERT(BIT, 1),
	PRIMARY KEY(BlogCommentId),
	FOREIGN KEY(BlogId) REFERENCES Blog(BlogId),
	FOREIGN KEY(ApplicationUserId) REFERENCES ApplicationUser(ApplicationUserId)
)

/* CREATING THE TABLES*/
	CREATE INDEX [IX_ApplicationUser_NormalizedUsername] ON [dbo].[ApplicationUser] ([NormalizedUsername])
	CREATE INDEX [IX_ApplicationUser_NormalizedEmail] ON [dbo].[ApplicationUser] (NormalizedEmail)


/* CREATINT A SCHEMA: */
CREATE SCHEMA [aggregate];

/* CREATING VIEWS */

/* [1] */
CREATE VIEW [aggregate].[Blog]
	AS
	SELECT 
	t1.BlogId,
	t1.ApplicationUserId,
	t2.username,
	t1.Title,
	t1.Content,
	t1.PhotoId,
	t1.PublishDate,
	t1.UpdateDate,
	t1.ActivateInd
	FROM
		dbo.Blog t1
	INNER JOIN 
		dbo.ApplicationUser t2 ON t1.ApplicationUserId = t2.ApplicationUserId;

/* [2] */

CREATE VIEW [aggregate].[BlogComment]
	AS
	SELECT 
	t1.BlogCommentId,
	t1.ParentBlogCommentId,
	t1.BlogId,
	t1.Content,
	t2.Username,
	t2.ApplicationUserId,
	t1.PublishDate,
	t1.UpdateDate,
	t1.ActiveInd
	FROM
		dbo.BlogComment t1
	INNER JOIN 
		dbo.ApplicationUser t2 ON t1.ApplicationUserId = t2.ApplicationUserId;

/* CREATE TYPE*/
drop TYPE [dbo].[AccountType]
CREATE TYPE [dbo].[AccountType] AS TABLE
(
	[Username] VARCHAR(20) NOT NULL,
	[NormalizedUsername] VARCHAR(20) NOT NULL,
	[Email] VARCHAR(30) NOT NULL,
	[NormalizedEmail] VARCHAR(30) NOT NULL,
	[FullName] VARCHAR(30) NULL,
	[PasswordHash] NVARCHAR(MAX) NOT NULL
)


CREATE TYPE [dbo].[PhotoType] AS TABLE
(
	[PublicId] VARCHAR(50) NOT NULL,
	[ImageUrl] VARCHAR(250) NOT NULL,
	[Description] VARCHAR(30) NOT NULL
)


CREATE TYPE [dbo].[BlogType] AS TABLE
(
	[BlogId] INT NOT NULL,
	[Title] VARCHAR(50) NOT NULL,
	[Content] VARCHAR(MAX) NOT NULL,
	[PhotoId] INT  NULL
)

CREATE TYPE [dbo].[BlogCommentType] AS TABLE
(
	[BlogCommentId] INT NOT NULL,
	[PrentBlogCommentId] INT NULL,
	[BlogId] INT NOT NULL,
	[Content] VARCHAR(300) NOT NULL
)

/*CREATING THE STORE PROCEDURES*/
CREATE PROCEDURE [dbo].[Account_GetByUsername]
	@NormalizedUsername VARCHAR(20)
	AS
		SELECT
			[ApplicationUserId],
			[Username],
			[NormalizedUsername],
			[Email],
			[NormalizedEmail],
			[FullName],
			[PasswordHash]
		FROM [dbo].[ApplicationUser] t1
		WHERE t1.[NormalizedUsername] = @NormalizedUsername;
		

CREATE PROCEDURE [dbo].[Account_Insert]
	@Account AccountType READONLY		
	AS	
	INSERT INTO [dbo].[ApplicationUser]
           ([Username]
           ,[NormalizedUsername]
           ,[Email]
           ,[NormalizedEmail]
           ,[FullName]
           ,[PasswordHash])
	SELECT 
		[Username]
		,[NormalizedUsername]
        ,[Email]
        ,[NormalizedEmail]
        ,[FullName]
        ,[PasswordHash]	
	FROM
		@Account 

		SELECT CAST(SCOPE_IDENTITY() AS INT); /* RETURN THE LAST IDENTITY VALUE INSERTED INTO THE COLUMN*/
     

CREATE PROCEDURE [dbo].[Blog_Delete]
	@BlogId INT		
	AS	
	UPDATE [dbo].[BlogComment]
		SET 
		[ActiveInd] = CONVERT(BIT, 0)
		WHERE [BlogId] = @BlogId

	UPDATE [dbo].[Blog]
		SET
			[PhotoId] = NULL,
			[ActivateInd] = CONVERT(BIT, 0)
		WHERE
			[BlogId] = @BlogId

CREATE PROCEDURE [dbo].[Blog_Get]
	@BlogId INT
	AS
	SELECT 
		[BlogId]
		,[ApplicationUserId]
		,[username]
		,[Title]
		,[Content]
		,[PhotoId]
		,[PublishDate]
		,[UpdateDate]
		,[ActivateInd]
	FROM 
		[aggregate].[Blog] t1
	WHERE 
		t1.[BlogId] = @BlogId AND
		t1.ActivateInd = CONVERT(BIT, 1)


CREATE PROCEDURE [dbo].[Blog_GetAll]
	@Offset INT,
	@PageSize INT
	AS
		SELECT 
		[BlogId]
		,[ApplicationUserId]
		,[username]
		,[Title]
		,[Content]
		,[PhotoId]
		,[PublishDate]
		,[UpdateDate]
		,[ActivateInd]
	FROM 
		[aggregate].[Blog] t1
	WHERE 
		t1.[ActivateInd] = CONVERT(BIT,1)
	ORDER BY 
		t1.[BlogId]
	OFFSET @Offset ROWS
	FETCH NEXT @PageSize ROWS ONLY;

	SELECT COUNT(*) 
	FROM 
		[aggregate].[Blog] t1 
	WHERE 
		t1.[ActivateInd] = CONVERT(BIT, 1);



CREATE PROCEDURE [dbo].[Blog_GetAllFamous]
	AS
	SELECT
		t1.[BlogId]
		,t1.[ApplicationUserId]
		,t1.[PhotoId]
		,t1.[Title]
		,t1.[Content]
		,t1.[PublishDate]
		,t1.[UpdateDate]
		,t1.[ActivateInd]
	FROM 
	[aggregate].[Blog] t1
	INNER JOIN
		[dbo].[BlogComment] t2 ON t1.[BlogId]= t2.[BlogId]
	WHERE 
		t1.ActivateInd	= CONVERT(BIT, 1) AND
		t2.ActiveInd	= CONVERT(BIT, 1)
	GROUP BY 
		 t1.[BlogId]
		,t1.[ApplicationUserId]
		,t1.[PhotoId]
		,t1.[Title]
		,t1.[Content]
		,t1.[PublishDate]
		,t1.[UpdateDate]
		,t1.[ActivateInd]
	ORDER BY 
		COUNT(t2.ParentBlogCommentId)
	DESC





	
	
  


