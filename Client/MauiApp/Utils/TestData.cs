using Medflix.Models.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medflix.Utils
{
    public static class TestData
    {
        public static MediaDetails Media = new MediaDetails
        {
            Id = "1234",
            ImdbId = "tt12345",
            Title = "Deadpool & Wolverine",
            CoverImageUrl = "https://image.tmdb.org/t/p/original/1Qb4L5L8UUWj9nRf6p779q27S9O.jpg",
            YoutubeTrailerUrl = "https://youtube.com/embed/",
            Duration = 7378,
            Genres = new Genre[] 
            {
                new Genre { Id = 12, Name ="Action"},
                new Genre { Id = 123, Name ="Thriller"},
                new Genre { Id = 12, Name ="Comedy"},
            },
            Year = 2024,
            Director = "Christopher Nolan",
            Cast = "Brad Pitt, George Clooney, Emma stone, Edward Norton, Eddie Alvarez",
            Synopsis = "L’incroyable épopée d'un robot -- l'unité ROZZUM 7134 alias \"Roz\" -- qui après avoir fait naufrage sur une île déserte doit apprendre à s'adapter à un environnement hostile en nouant petit à petit des relations avec les animaux de l'île. Il finit par adopter le petit d’une oie, un oison, qui se retrouve orphelin.",
            BackgroundImageUrl = "https://image.tmdb.org/t/p/original/f6TCICUC8OSBtZDKgg18T6PjfIM.jpg",
            LogoImageUrl = "https://image.tmdb.org/t/p/original/63tQ41DjyWokH4b6kqL7Umnf3PP.png",
            Rating = 5.8,
            SeasonsCount = 4
        };

        public static List<MediaDetails> Medias = new List<MediaDetails> { Media, Media, Media, Media, Media, Media, Media, Media, Media, Media, Media, Media, Media, Media, Media, Media, Media, Media, Media, Media };

        public static List<Episode> Episodes = new List<Episode>()
            {
                new Episode
                {
                    EpisodeNumber = 1,
                    ImagePath = "https://image.tmdb.org/t/p/original/v0HhlqpPPv6v3ok5RMFEPL66qPD.jpg",
                    Name = "The thing of true",
                    Overview = "dcsdvjsdv sdvposdjvposdvjsdpo sdvsdoivkosdvkpdsovks sdvposdvosdpoopod dsvsdvsdpovksdovksdkvodsvs ",
                    RunTime = 3210
                },
                 new Episode
                {
                    EpisodeNumber = 2,
                    ImagePath = "https://image.tmdb.org/t/p/original/v0HhlqpPPv6v3ok5RMFEPL66qPD.jpg",
                    Name = "The thing of true",
                    Overview = "dcsdvjsdv sdvposdjvposdvjsdpo sdvsdoivkosdvkpdsovks sdvposdvosdpoopod dsvsdvsdpovksdovksdkvodsvs ",
                    RunTime = 3210
                },
                  new Episode
                {
                    EpisodeNumber = 3,
                    ImagePath = "https://image.tmdb.org/t/p/original/v0HhlqpPPv6v3ok5RMFEPL66qPD.jpg",
                    Name = "The thing of true",
                    Overview = "dcsdvjsdv sdvposdjvposdvjsdpo sdvsdoivkosdvkpdsovks sdvposdvosdpoopod dsvsdvsdpovksdovksdkvodsvs ",
                    RunTime = 3210
                },
                   new Episode
                {
                    EpisodeNumber = 4,
                    ImagePath = "https://image.tmdb.org/t/p/original/v0HhlqpPPv6v3ok5RMFEPL66qPD.jpg",
                    Name = "The thing of true",
                    Overview = "dcsdvjsdv sdvposdjvposdvjsdpo sdvsdoivkosdvkpdsovks sdvposdvosdpoopod dsvsdvsdpovksdovksdkvodsvs ",
                    RunTime = 3210
                },
                    new Episode
                {
                    EpisodeNumber = 5,
                    ImagePath = "https://image.tmdb.org/t/p/original/v0HhlqpPPv6v3ok5RMFEPL66qPD.jpg",
                    Name = "The thing of true",
                    Overview = "dcsdvjsdv sdvposdjvposdvjsdpo sdvsdoivkosdvkpdsovks sdvposdvosdpoopod dsvsdvsdpovksdovksdkvodsvs ",
                    RunTime = 3210
                },
                        new Episode
                {
                    EpisodeNumber = 6,
                    ImagePath = "https://image.tmdb.org/t/p/original/v0HhlqpPPv6v3ok5RMFEPL66qPD.jpg",
                    Name = "The thing of true",
                    Overview = "dcsdvjsdv sdvposdjvposdvjsdpo sdvsdoivkosdvkpdsovks sdvposdvosdpoopod dsvsdvsdpovksdovksdkvodsvs ",
                    RunTime = 3210
                },
                   new Episode
                {
                    EpisodeNumber = 7,
                    ImagePath = "https://image.tmdb.org/t/p/original/v0HhlqpPPv6v3ok5RMFEPL66qPD.jpg",
                    Name = "The thing of true",
                    Overview = "dcsdvjsdv sdvposdjvposdvjsdpo sdvsdoivkosdvkpdsovks sdvposdvosdpoopod dsvsdvsdpovksdovksdkvodsvs ",
                    RunTime = 3210
                },
                    new Episode
                {
                    EpisodeNumber = 8,
                    ImagePath = "https://image.tmdb.org/t/p/original/v0HhlqpPPv6v3ok5RMFEPL66qPD.jpg",
                    Name = "The thing of true",
                    Overview = "dcsdvjsdv sdvposdjvposdvjsdpo sdvsdoivkosdvkpdsovks sdvposdvosdpoopod dsvsdvsdpovksdovksdkvodsvs ",
                    RunTime = 3210
                },
                new Episode
                {
                    EpisodeNumber = 9,
                    ImagePath = "https://image.tmdb.org/t/p/original/v0HhlqpPPv6v3ok5RMFEPL66qPD.jpg",
                    Name = "The thing of true",
                    Overview = "dcsdvjsdv sdvposdjvposdvjsdpo sdvsdoivkosdvkpdsovks sdvposdvosdpoopod dsvsdvsdpovksdovksdkvodsvs ",
                    RunTime = 3210
                },
                   new Episode
                {
                    EpisodeNumber = 10,
                    ImagePath = "https://image.tmdb.org/t/p/original/v0HhlqpPPv6v3ok5RMFEPL66qPD.jpg",
                    Name = "The thing of true",
                    Overview = "dcsdvjsdv sdvposdjvposdvjsdpo sdvsdoivkosdvkpdsovks sdvposdvosdpoopod dsvsdvsdpovksdovksdkvodsvs ",
                    RunTime = 3210
                },
                    new Episode
                {
                    EpisodeNumber = 11,
                    ImagePath = "https://image.tmdb.org/t/p/original/v0HhlqpPPv6v3ok5RMFEPL66qPD.jpg",
                    Name = "The thing of true",
                    Overview = "dcsdvjsdv sdvposdjvposdvjsdpo sdvsdoivkosdvkpdsovks sdvposdvosdpoopod dsvsdvsdpovksdovksdkvodsvs ",
                    RunTime = 3210
                }
            };

        public static List<MediaTorrent> MediaTorrents = new List<MediaTorrent>() { new MediaTorrent { DownloadUrl = "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/ElephantsDream.mp4", Quality = "1080p" }, new MediaTorrent { DownloadUrl = "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4", Quality = "720p" } };

        public static WatchMediaInfo WatchMedia = new WatchMediaInfo
        {
            Media = Media,
            SeasonNumber = 2,
            EpisodeNumber = 23,
            CurrentTime = 2000,
            TotalDuration = 4213,
            TorrentUrl = "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4"
        };

        public static List<WatchMediaInfo> WatchMedias = new List<WatchMediaInfo>() { WatchMedia, WatchMedia, WatchMedia, WatchMedia, WatchMedia, WatchMedia, WatchMedia, WatchMedia, WatchMedia, WatchMedia, WatchMedia };

        public static string Mp4Url = "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4";
        public static string TorrentUrl = "https://raw.githubusercontent.com/webtorrent/webtorrent-fixtures/refs/heads/master/fixtures/bunny.torrent";
    }
}

