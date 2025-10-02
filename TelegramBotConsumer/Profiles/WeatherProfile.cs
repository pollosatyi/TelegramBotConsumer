using AutoMapper;
using TelegramBotConsumer.DTOs;
using WeatherTelegramBot.DTOs;
using WeatherTelegramBot.Models;
using static WeatherTelegramBot.DTOs.CreateModelDto;


namespace WeatherTelegramBot.Profiles
{
    public class WeatherProfile : Profile
    {
        public WeatherProfile()
        {

            //Маппинг из сложного WeatherDto в простую WeatherModel
            CreateMap<CreateModelDto, WeatherModel>()
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Name.ToLower()))
                .ForMember(dest => dest.Temperature, opt => opt.MapFrom(src => src.Main.Temp))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Weather.First().Description))
                .ForMember(dest => dest.WindSpeed, opt => opt.MapFrom(src => src.Wind.Speed))
                .ForMember(dest => dest.RequestCount, opt => opt.MapFrom(_ => 1))
                .ForMember(dest => dest.AverageTemperature, opt => opt.MapFrom(src => src.Main.Temp));

            CreateMap<WeatherModel, ReadModelDto>()
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => char.ToUpper(src.City[0]) + src.City.Substring(1)))
                .ForMember(dest => dest.Temperature, opt => opt.MapFrom(src => src.Temperature))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.WindSpeed, opt => opt.MapFrom(src => src.WindSpeed));

            CreateMap<WeatherModel, ReadStaticticModel>()
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => char.ToUpper(src.City[0]) + src.City.Substring(1)))
                .ForMember(dest => dest.RequestCount, opt => opt.MapFrom(src => src.RequestCount))
                .ForMember(dest => dest.AverageTemperature, opt => opt.MapFrom(src => src.AverageTemperature));



        }
    }
}
