ALTER TABLE [dbo].[Camera]  ALTER COLUMN [RtspPort] [nvarchar](150)

ALTER TABLE  [dbo].[Camera] 
ADD [VideoDisabled] [bit] NOT NULL default 0;

ALTER TABLE  [dbo].[Camera] 
ADD [Presets] [nvarchar](500) NOT NULL default '20;';