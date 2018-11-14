USE [UHUB_DB]
GO
INSERT [dbo].[EntChildMap] ([ParentEntType], [ChildEntType], [Description]) VALUES (2, 1, N'School -> User')
INSERT [dbo].[EntChildMap] ([ParentEntType], [ChildEntType], [Description]) VALUES (2, 3, N'School -> SchoolMajor')
INSERT [dbo].[EntChildMap] ([ParentEntType], [ChildEntType], [Description]) VALUES (2, 4, N'School -> SchoolClub')
INSERT [dbo].[EntChildMap] ([ParentEntType], [ChildEntType], [Description]) VALUES (2, 6, N'School -> Post')
INSERT [dbo].[EntChildMap] ([ParentEntType], [ChildEntType], [Description]) VALUES (4, 5, N'SchoolClub -> SchoolClubModerator')
INSERT [dbo].[EntChildMap] ([ParentEntType], [ChildEntType], [Description]) VALUES (4, 6, N'SchoolClub -> Post')
INSERT [dbo].[EntChildMap] ([ParentEntType], [ChildEntType], [Description]) VALUES (6, 7, N'Post -> Comment')
INSERT [dbo].[EntChildMap] ([ParentEntType], [ChildEntType], [Description]) VALUES (6, 8, N'Post -> File')
INSERT [dbo].[EntChildMap] ([ParentEntType], [ChildEntType], [Description]) VALUES (6, 9, N'Post -> Image')
INSERT [dbo].[EntChildMap] ([ParentEntType], [ChildEntType], [Description]) VALUES (7, 7, N'Comment -> Comment')
INSERT [dbo].[EntChildMap] ([ParentEntType], [ChildEntType], [Description]) VALUES (7, 8, N'Comment -> File')
INSERT [dbo].[EntChildMap] ([ParentEntType], [ChildEntType], [Description]) VALUES (7, 9, N'Comment -> Image')
