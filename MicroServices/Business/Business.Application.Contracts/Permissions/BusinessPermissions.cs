using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Permissions
{
    public static class BusinessPermissions
    {
        public const string Business = "Business";

        public static class Book
        {
            public const string Default = Business + ".Book";
            public const string Delete = Default + ".Delete";
            public const string Update = Default + ".Update";
            public const string Create = Default + ".Create";
        }

        public static class PrintTemplate
        {
            public const string Default = Business + ".PrintTemplate";
            public const string Delete = Default + ".Delete";
            public const string Update = Default + ".Update";
            public const string Create = Default + ".Create";
        }
        
        public static class TaskItem
        {
            public const string Default = Business + ".TaskItem";
            public const string Delete = Default + ".Delete";
            public const string Update = Default + ".Update";
            public const string Create = Default + ".Create";
        }
        
        public static class TaskCategory
        {
            public const string Default = Business + ".TaskCategory";
            public const string Delete = Default + ".Delete";
            public const string Update = Default + ".Update";
            public const string Create = Default + ".Create";
        }
        
        public static class TaskDashboard
        {
            public const string Default = Business + ".TaskDashboard";
            public const string Delete = Default + ".Delete";
            public const string Update = Default + ".Update";
            public const string Create = Default + ".Create";
        }
        
        public static class TaskOrganization
        {
            public const string Default = Business + ".TaskOrganization";
            public const string Delete = Default + ".Delete";
            public const string Update = Default + ".Update";
            public const string Create = Default + ".Create";
        }

        //Code generation...
    }
}
