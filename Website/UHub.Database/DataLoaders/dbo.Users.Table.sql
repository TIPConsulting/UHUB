USE [UHUB_DB]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 11/11/2018 6:36:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


GO
INSERT [dbo].[Users] ([EntID], [RefUID], [Email], [Domain], [Username], [IsConfirmed], [IsApproved], [Version], [IsAdmin]) VALUES (0, N'FF74F1DC5C4C47AFA88D60703ED447DCC14E6A22FFE549E49B6A9A82E4D8234811AD4AA97B1E480E9CECD4CD2D5129B2', N'system@test.test', N'@test.test', N'system-3C17896E-3F70-4D6B-93BF-D7D1B07B5C91-8B011FD7-A748-468A-9AF9-855625F9C33C', 1, 1, N'P(TGCFR&@#()1ojf32', 1)
INSERT [dbo].[Users] ([EntID], [RefUID], [Email], [Domain], [Username], [IsConfirmed], [IsApproved], [Version], [IsAdmin]) VALUES (9, N'F48CCEE1EC5C4DF1A271BAB4127E08554BBCFA4ADADA42BF81F50D94A92937A4F9E11BA9226B4850A17CE59FFB22A45C', N'aual1780@colorado.edu', N'@colorado.edu', N'admin', 1, 1, N'DCU#*_[jfy08923', 0)
ALTER TABLE [dbo].[Users] ADD  CONSTRAINT [DF_Users_RefUID]  DEFAULT (replace(concat(newid(),newid(),newid()),'-','')) FOR [RefUID]
GO

GO

GO

GO

GO

GO
