--USE [ClickHealWealDB]
--GO
/****** Object:  Table [dbo].[CityMst]    Script Date: 05-08-2022 16:17:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CityMst](
	[CityId] [int] NULL,
	[StateId] [int] NULL,
	[CityName] [nvarchar](250) NULL,
	[Latitude] [nvarchar](250) NULL,
	[Longitude] [nvarchar](250) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CountryMst]    Script Date: 05-08-2022 16:17:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CountryMst](
	[CountryId] [int] NULL,
	[CountryName] [nvarchar](250) NULL,
	[Iso3] [nvarchar](250) NULL,
	[Iso2] [nvarchar](250) NULL,
	[NumericCode] [nvarchar](250) NULL,
	[PhoneCode] [nvarchar](250) NULL,
	[Capital] [nvarchar](250) NULL,
	[Currency] [nvarchar](250) NULL,
	[CurrencyName] [nvarchar](250) NULL,
	[CurrencySymbol] [nvarchar](250) NULL,
	[Tld] [nvarchar](250) NULL,
	[Native] [nvarchar](250) NULL,
	[Region] [nvarchar](250) NULL,
	[SubRegion] [nvarchar](250) NULL,
	[Latitude] [nvarchar](250) NULL,
	[Longitude] [nvarchar](250) NULL,
	[Emoji] [nvarchar](250) NULL,
	[EmojiU] [nvarchar](250) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StateMst]    Script Date: 05-08-2022 16:17:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StateMst](
	[StateId] [int] NULL,
	[CountryId] [int] NULL,
	[StateName] [nvarchar](250) NULL,
	[StateCode] [nvarchar](250) NULL,
	[Latitude] [nvarchar](250) NULL,
	[Longitude] [nvarchar](250) NULL
) ON [PRIMARY]
GO
