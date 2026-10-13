using EventFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Application.DTOs
{
    public class TicketDto
    {
        public Guid Id { get; set; }
        public Guid TicketId { get; set; }    
        public Guid EventId { get; set; }
        public Guid UserId { get; set; }
        public string TicketNumber { get; set; } = null!;
        public TicketStatus Status { get; set; }
        public DateTime TicketCreationDate { get; set; }
        public DateTime? LastModificationDate { get; set; }

        public decimal Price { get; set; }

        public string? SeatNumber { get; set; }
        public string? RowNumber { get; set; }
        public string? Sector { get; set; }



    }
}
