USE [master]
GO
/****** Object:  Database [demo]    Script Date: 15-01-2023 18:08:30 ******/
CREATE DATABASE [demo]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'demo', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\DATA\demo.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'demo_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\DATA\demo_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO
ALTER DATABASE [demo] SET COMPATIBILITY_LEVEL = 150
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [demo].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [demo] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [demo] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [demo] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [demo] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [demo] SET ARITHABORT OFF 
GO
ALTER DATABASE [demo] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [demo] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [demo] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [demo] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [demo] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [demo] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [demo] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [demo] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [demo] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [demo] SET  ENABLE_BROKER 
GO
ALTER DATABASE [demo] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [demo] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [demo] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [demo] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [demo] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [demo] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [demo] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [demo] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [demo] SET  MULTI_USER 
GO
ALTER DATABASE [demo] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [demo] SET DB_CHAINING OFF 
GO
ALTER DATABASE [demo] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [demo] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [demo] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [demo] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [demo] SET QUERY_STORE = OFF
GO
USE [demo]
GO
/****** Object:  Table [dbo].[Employee]    Script Date: 15-01-2023 18:08:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Employee](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EmployeeName] [varchar](200) NOT NULL,
	[ContactNumber] [varchar](15) NOT NULL,
	[CreatedOn] [datetime] NULL,
	[IsDeleted] [bit] NULL,
	[IsVerified] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[ContactNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EmployeeOTP]    Script Date: 15-01-2023 18:08:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EmployeeOTP](
	[EmployeeId] [int] NULL,
	[OTP] [varchar](10) NULL,
	[CreatedOn] [datetime] NULL,
	[IsDeleted] [bit] NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Employee] ADD  DEFAULT (getutcdate()) FOR [CreatedOn]
GO
ALTER TABLE [dbo].[Employee] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[Employee] ADD  DEFAULT ((0)) FOR [IsVerified]
GO
ALTER TABLE [dbo].[EmployeeOTP] ADD  DEFAULT (getutcdate()) FOR [CreatedOn]
GO
ALTER TABLE [dbo].[EmployeeOTP] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[EmployeeOTP]  WITH CHECK ADD FOREIGN KEY([EmployeeId])
REFERENCES [dbo].[Employee] ([Id])
GO
/****** Object:  StoredProcedure [dbo].[insert_employee]    Script Date: 15-01-2023 18:08:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE proc [dbo].[insert_employee]
(
	@EmployeeName varchar(200),
	@ContactNumber varchar(15),
	@OTP varchar(10)
)
as
Begin

	declare @Status bit, @Message varchar(200);
	IF EXISTS(Select 1 from [dbo].[Employee] 
				where ContactNumber=@ContactNumber 
				and ISNULL(IsDeleted,0)=0 
				and ISNULL(IsVerified,0)=1)
	Begin
		Set @Status=0;
		Set @Message='Record already exists.';
	End
	Else IF EXISTS(Select 1 from [dbo].[Employee] 
				where ContactNumber=@ContactNumber 
				and ISNULL(IsDeleted,0)=0
				and ISNULL(IsVerified,0)=0)
	Begin
		
		declare @employeeId int=(Select ID from [dbo].[Employee] 
									where ContactNumber=@ContactNumber 
									and ISNULL(IsDeleted,0)=0 
									);
		-- Update all the OTP for the user as deleted so the validation will happen for new OTP
		update [dbo].[EmployeeOTP]
		Set IsDeleted=1
		where EmployeeId=@employeeId

		insert into EmployeeOTP(EmployeeId,OTP)
		values(@employeeId,@OTP);

		Set @Status=1;
		Set @Message='The user exist but not verified.';
	End
	Else
	Begin
		Begin Tran
			Begin Try
				insert into Employee(EmployeeName,ContactNumber)
				values(@EmployeeName,@ContactNumber);

				declare @insertedId int=SCOPE_IDENTITY();

				insert into EmployeeOTP(EmployeeId,OTP)
				values(@insertedId,@OTP);

				Set @Status=1;
				Set @Message='Record inserted successfully.';
				COMMIT;
			End Try
			Begin Catch
				ROLLBACK;

				Set @Status=0;
				Set @Message='Error occurred while inserting the record.';
			End Catch
	End

	Select @Status as [Status], @Message as [Message]
End
GO
/****** Object:  StoredProcedure [dbo].[login_employee]    Script Date: 15-01-2023 18:08:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[login_employee]
(
	@ContactNumber varchar(15),
	@OTP varchar(10)
)
as
Begin
	
	declare @Status bit, @Message varchar(200);
	IF EXISTS(Select 1 from [dbo].[Employee] 
				where ContactNumber=@ContactNumber 
				and ISNULL(IsDeleted,0)=0 
				and ISNULL(IsVerified,0)=1)
	Begin
		IF (Select top 1 OTP from [dbo].[Employee] e 
			inner join [dbo].[EmployeeOTP] otp on e.Id=otp.EmployeeId
			where ISNULL(e.IsDeleted,0)=0
			and ISNULL(otp.IsDeleted,0)=0
			order by otp.CreatedOn desc
			)=@OTP
		Begin
			Begin Tran
				Begin Try
					
					update [dbo].[EmployeeOTP]
					Set IsDeleted=1
					where OTP=@OTP

					Set @Status=1;
					Set @Message='User verified successfully.';

					COMMIT;
				End Try
				Begin Catch
					ROLLBACK;
					Set @Status=0;
					Set @Message='Error occurred while verifying the user.';
				End Catch
			
		End
		Else
		Begin
			Set @Status=0;
			Set @Message='Wrong OTP provided.';
		End
	End
	Else IF EXISTS(Select 1 from [dbo].[Employee] 
				where ContactNumber=@ContactNumber 
				and ISNULL(IsDeleted,0)=0
				and ISNULL(IsVerified,0)=0)
	Begin
		Set @Status=0;
		Set @Message='The user exist but not verified.';
	End
	Else
	Begin
		Set @Status=0;
		Set @Message='The user does not exist or not verified.';
	End

	Select @Status as [Status], @Message as [Message]
End
GO
/****** Object:  StoredProcedure [dbo].[save_otp]    Script Date: 15-01-2023 18:08:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[save_otp]
(
	@ContactNumber varchar(15),
	@OTP varchar(10)
)
as
Begin
	
	declare @Status bit, @Message varchar(200);
	IF EXISTS(Select 1 from [dbo].[Employee] 
				where ContactNumber=@ContactNumber 
				and ISNULL(IsDeleted,0)=0 
				and ISNULL(IsVerified,0)=1)
	Begin
		
		Begin Tran
			Begin Try
					
				declare @employeeId int=(Select ID from [dbo].[Employee] 
								where ContactNumber=@ContactNumber 
								and ISNULL(IsDeleted,0)=0 
								);
					
				-- Update all the OTP for the user as deleted so the validation will happen for new OTP
				update [dbo].[EmployeeOTP]
				Set IsDeleted=1
				where EmployeeId=@employeeId

				-- New OTP
				insert into EmployeeOTP(EmployeeId,OTP)
				values(@employeeId,@OTP);

				Set @Status=1;
				Set @Message='OTP saved successfully.';

				COMMIT;
			End Try
			Begin Catch
				ROLLBACK;
				Set @Status=0;
				Set @Message='Error occurred while verifying the user.';
			End Catch
			
	End
	Else IF EXISTS(Select 1 from [dbo].[Employee] 
				where ContactNumber=@ContactNumber 
				and ISNULL(IsDeleted,0)=0
				and ISNULL(IsVerified,0)=0)
	Begin
		Set @Status=0;
		Set @Message='The user exist but not verified.';
	End
	Else
	Begin
		Set @Status=0;
		Set @Message='The user does not exist or not verified.';
	End

	Select @Status as [Status], @Message as [Message]
End
GO
/****** Object:  StoredProcedure [dbo].[verify_employee]    Script Date: 15-01-2023 18:08:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[verify_employee]
(
	@ContactNumber varchar(15),
	@OTP varchar(10)
)
as
Begin
	
	declare @Status bit, @Message varchar(200);
	IF EXISTS(Select 1 from [dbo].[Employee] 
				where ContactNumber=@ContactNumber 
				and ISNULL(IsDeleted,0)=0)
	Begin
		IF (Select top 1 OTP from [dbo].[Employee] emp
			inner join [dbo].[EmployeeOTP] otp on emp.Id=otp.EmployeeId
			where ISNULL(emp.IsDeleted,0)=0
			and ISNULL(otp.IsDeleted,0)=0
			and ContactNumber=@ContactNumber
			order by otp.CreatedOn desc
			)=@OTP
		Begin
			Begin Tran
				Begin Try
					update [dbo].[Employee]
					Set IsVerified=1
					where ContactNumber=@ContactNumber

					update [dbo].[EmployeeOTP]
					Set IsDeleted=1
					where OTP=@OTP

					Set @Status=1;
					Set @Message='User verified successfully.';

					COMMIT;
				End Try
				Begin Catch
					ROLLBACK;
					Set @Status=0;
					Set @Message='Error occurred while verifying the user.';
				End Catch
			
		End
		Else
		Begin
			Set @Status=0;
			Set @Message='Wrong OTP provided.';
		End
	End
	Else
	Begin
		Set @Status=0;
		Set @Message='The user does not exist.';
	End

	Select @Status as [Status], @Message as [Message]
End
GO
USE [master]
GO
ALTER DATABASE [demo] SET  READ_WRITE 
GO
