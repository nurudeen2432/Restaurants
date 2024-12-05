﻿namespace Restaurants.Domain.Exceptions;

public class NotFoundExceptions(string resourceType, string resourceIdentifier) : 
    Exception($"{resourceType} with id: {resourceIdentifier} doesn't exist".Trim()) 
{

    //public NotFoundExceptions(string message): base(message)
    //{
        
    //}
}
