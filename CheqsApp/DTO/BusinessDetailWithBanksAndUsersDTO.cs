﻿using CheqsApp.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CheqsApp.DTO
{
    public class BusinessDetailWithBanksAndUsersDTO
    {
        public required BusinessSimpleDTO Business { get; set; }
        public List<BankDetailWithUsersDTO> Banks { get; set; } = new List<BankDetailWithUsersDTO>();

    }
}
