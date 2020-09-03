using Services.Dto;

namespace Services.Responses
{
    public class PieResponse
    {
        public int StatusCodeHttp { get; set; }

        public PieRecord PieRecord { get; set; }
    }
}