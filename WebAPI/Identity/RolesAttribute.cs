﻿using Microsoft.AspNetCore.Authorization;

public class RolesAttribute : AuthorizeAttribute
{
    public RolesAttribute(params string[] roles): base()
    {
        Roles = string.Join(",", roles);
    }
}