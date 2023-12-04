﻿using System;

namespace LegoBuilder.Exceptions
{
    public class IncorrectEntryException : Exception
    {
        // created for when we want to throw an exception for a bad user input
        public IncorrectEntryException() : base() { }
        public IncorrectEntryException(string message) : base(message) { }
        public IncorrectEntryException(string message, Exception inner) : base(message, inner) { }
    }
}
