using CheqsApp.Models;
using System.ComponentModel.DataAnnotations;

namespace CheqsApp.DTO
{
    public class BusinessDetailWithBanksAndUsersDTO
    {
        public required BusinessSimpleDTO Business { get; set; }
        public List<BankDetailWithUsersDTO> Banks { get; set; } = new List<BankDetailWithUsersDTO>();
    }
}
