using Business.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace Business.Permissions
{
    public class BusinessPermissionDefinitionProvider: PermissionDefinitionProvider
    {

        public override void Define(IPermissionDefinitionContext context)
        {
            var Business = context.AddGroup(BusinessPermissions.Business, L("Business"));

            var Book = Business.AddPermission(BusinessPermissions.Book.Default, L("Book"));
            Book.AddChild(BusinessPermissions.Book.Update, L("Edit"));
            Book.AddChild(BusinessPermissions.Book.Delete, L("Delete"));
            Book.AddChild(BusinessPermissions.Book.Create, L("Create"));

            var PrintTemplate = Business.AddPermission(BusinessPermissions.PrintTemplate.Default, L("PrintTemplate"));
            PrintTemplate.AddChild(BusinessPermissions.PrintTemplate.Update, L("Edit"));
            PrintTemplate.AddChild(BusinessPermissions.PrintTemplate.Delete, L("Delete"));
            PrintTemplate.AddChild(BusinessPermissions.PrintTemplate.Create, L("Create"));

            //Code generation...
            
            var Item = Business.AddPermission(BusinessPermissions.TaskItem.Default, L("TaskItem"));
            Item.AddChild(BusinessPermissions.TaskItem.Update, L("Edit"));
            Item.AddChild(BusinessPermissions.TaskItem.Delete, L("Delete"));
            Item.AddChild(BusinessPermissions.TaskItem.Create, L("Create"));
            
            var Category = Business.AddPermission(BusinessPermissions.TaskCategory.Default, L("TaskCategory"));
            Category.AddChild(BusinessPermissions.TaskCategory.Update, L("Edit"));
            Category.AddChild(BusinessPermissions.TaskCategory.Delete, L("Delete"));
            Category.AddChild(BusinessPermissions.TaskCategory.Create, L("Create"));
            
            var Dashboard = Business.AddPermission(BusinessPermissions.TaskDashboard.Default, L("TaskDashboard"));
            Dashboard.AddChild(BusinessPermissions.TaskDashboard.Update, L("Edit"));
            Dashboard.AddChild(BusinessPermissions.TaskDashboard.Delete, L("Delete"));
            Dashboard.AddChild(BusinessPermissions.TaskDashboard.Create, L("Create"));
            
            var organization = Business.AddPermission(BusinessPermissions.TaskOrganization.Default, L("TaskOrganization"));
            organization.AddChild(BusinessPermissions.TaskOrganization.Update, L("Edit"));
            organization.AddChild(BusinessPermissions.TaskOrganization.Delete, L("Delete"));
            organization.AddChild(BusinessPermissions.TaskOrganization.Create, L("Create"));
        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<BusinessResource>(name);
        }
    }
}
