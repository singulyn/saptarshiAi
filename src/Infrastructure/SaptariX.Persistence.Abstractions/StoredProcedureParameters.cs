using System.Dynamic;

namespace SaptariX.Persistence.Abstractions;

public static class StoredProcedureParameters
{
    public static object WithOrganization(Guid organizationId, object? parameters = null)
    {
        IDictionary<string, object?> result = new ExpandoObject();
        result["OrganizationId"] = organizationId;

        if (parameters is not null)
        {
            foreach (var property in parameters.GetType().GetProperties())
            {
                result[property.Name] = property.GetValue(parameters);
            }
        }

        return result;
    }
}
