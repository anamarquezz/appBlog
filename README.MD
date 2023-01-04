C#6 Notes.

The base code have -> Startu.cs


---ANGULAR---

::commands::
1. install angular, bootstrap, fontawesome, 
|   npm install -g @angular/cli |
|   ng new BlogLab-UI --skip-git --skip-tests --skip-install |
|   npm install         |
|   ng add ngx-bootstrap   |
|   npm install ngx-bootstrap@latest --legacy-peer-deps |
|   npm install --save @fortawesome/fontawesome-free |
Change Version:
npm uninstall -g @angular/cli
npm cache cleanc
npm install -g @angular/cli@14.0.4

2. Add in angular.json
    a) in   "styles": []     
        > "./node_modules/@fortawesome/fontawesome-free/css/all.css"

3. install Extension Angular,
4. g serve, for runn
5. visual studio gitignore
    https://github.com/github/gitignore/blob/main/VisualStudio.gitignore




________________________________________________________________________________
|--------------------- IMPORTANT COMMANDS FOR ANGULAR ---------------------    |

1. CREATE NEW APP:
    ng new BlogLab-UI --skip-git --skip-tests --skip-install
2. RUN APP:
    ng serve
3. 


|______________________________________________________________________________|



|_______________________________________________________________________________|
|-----------------------------------  DB ---------------------------------------|

1. Create a data base 
    > CREATE DATABASE BlogDB;
2. Use that DB
    > USE BlogDB;
3. CREATE TABLES
    > ApplicationUser:
        CREATE TABLE ApplicationUser (
	        ApplicationUserId INT NOT NULL IDENTITY(1, 1),
	        Username VARCHAR(20) NOT NULL,
	        NormalizedUsername VARCHAR(20) NOT NULL,  --- ASPNET-IDENTITY MODEL | REGISTRATION & EMAIL | lookups Identify the user
	        Email VARCHAR(30) NOT NULL,
	        NormalizedEmail VARCHAR(30) NOT NULL,     --- ASPNET-IDENTITY MODEL | REGISTRATION & EMAIL | lookups Identify the user
	        FulName VARCHAR(30) NULL,
	        PasswordHash NVARCHAR(MAX) NOT NULL,
	        PRIMARY KEY(ApplicationUserId)
        )
    > Photo:
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
    > Blog
        CREATE TABLE Blog (
            BlogId INT NOT NULL IDENTITY(1, 1),
	        ApplicationUserId INT NOT NULL,
	        PhotoId INT NULL,
	        Title VARCHAR(50) NOT NULL,
	        Content VARCHAR(MAX) NOT NULL,
	        PublishDate DATETIME NOT NULL DEFAULT GETDATE(),
	        UpdateDate DATETIME NOT NULL DEFAULT GETDATE(),
	        ActivateInd BIT NOT NULL DEFAULT CONVERT(BIT, 1), -- boolean, activate or desactivate
	        PRIMARY KEY(blogId),
	        FOREIGN KEY(ApplicationUserId) REFERENCES ApplicationUser(ApplicationUserId),
	        FOREIGN KEY(PhotoId) REFERENCES Photo(PhotoId)
        )
    > BlogComment
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

4. CREATE INDEX'S
    > ApplicationUser:
        CREATE INDEX [IX_ApplicationUser_NormalizedUsername] ON [dbo].[ApplicationUser] ([NormalizedUsername])
        CREATE INDEX [IX_ApplicationUser_NormalizedEmail] ON [dbo].[ApplicationUser] (NormalizedEmail)

    > 

5. CREATING A SCHEMA:
    > CREATE SCHEMA [aggregate];

6. CREATING VIEWS
    > [aggregate].[Blog]:
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
	            INNER JOIN dbo.ApplicationUser t2 ON t1.ApplicationUserId = t2.ApplicationUserId;

    > [aggregate].[BlogComment]:
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
	            INNER JOIN dbo.ApplicationUser t2 ON t1.ApplicationUserId = t2.ApplicationUserId;

7. CREATING TYPES: //This is for create new ones,
    >  [dbo].[AccountType]:
        CREATE TYPE [dbo].[AccountType] AS TABLE
        (
	        [Username] VARCHAR(20) NOT NULL,
	        [NormalizedUsername] VARCHAR(20) NOT NULL,
	        [Email] VARCHAR(30) NOT NULL,
	        [NormalizedEmail] VARCHAR(30) NOT NULL,
	        [FullName] VARCHAR(30) NULL,
	        [PasswordHash] NVARCHAR(MAX) NOT NULL
        )
    > [dbo].[PhotoType]:
        CREATE TYPE [dbo].[PhotoType] AS TABLE
        (
	        [PublicId] VARCHAR(50) NOT NULL,
	        [ImageUrl] VARCHAR(250) NOT NULL,
	        [Description] VARCHAR(30) NOT NULL
        )
    >  [dbo].[BlogType]:
        CREATE TYPE [dbo].[BlogType] AS TABLE
        (
	        [BlogId] INT NOT NULL,
	        [Title] VARCHAR(50) NOT NULL,
	        [Content] VARCHAR(MAX) NOT NULL,
	        [PhotoId] INT  NULL
        )
    
    > [dbo].[BlogCommentType]:
        CREATE TYPE [dbo].[BlogCommentType] AS TABLE
        (
	        [BlogCommentId] INT NOT NULL,
	        [PrentBlogCommentId] INT NULL,
	        [BlogId] INT NOT NULL,
	        [Content] VARCHAR(300) NOT NULL
        )


8. CREATE PROCEDURES
    > [dbo].[Account_GetByUsername]:
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
    > [dbo].[Account_Insert]:
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
    > [dbo].[Blog_Delete]:
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
    
    > [dbo].[Blog_Get]:
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
    
    > [dbo].[Blog_GetAll]:
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

    > [dbo].[Blog_GetAllFamous]:
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
|______________________________________________________________________________|