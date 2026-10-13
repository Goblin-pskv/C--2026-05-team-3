using EventFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Application.DTOs
{
    public class PaymentDto
    {

        public Guid Id { get; set; }
        public Guid PaymentId { get; set; }
   
        public Guid TicketId { get; set; }
        public Guid UserId { get; set; }
        public decimal Price { get; set; }

        public PaymentStatus Status { get; set; }
        public DateTime PaymentCreationDate { get; set; }
        public DateTime? LastModificationDate { get; set; }
        public DateTime? PaidAt { get; set; }


    }
}
