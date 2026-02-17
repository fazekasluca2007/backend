using EcoTrip.Models.DtoS;

namespace EcoTrip.Services.IServices
{
    public interface IMail
    {
        void SendMail(SendMailDto sendMailDto);
    }
}
