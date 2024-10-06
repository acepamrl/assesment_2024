﻿namespace Event.Models.Request
{
    public class CreateEventRequest
    {
        public string Name { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Location { get; set; } = string.Empty;
        public double Price { get; set; }
        public int Ticket { get; set; }
    }
}