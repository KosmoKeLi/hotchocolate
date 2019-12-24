using System.Collections.Generic;
using HotChocolate.Types.Descriptors.Definitions;

namespace HotChocolate.Types.Filters.Conventions
{
    public class FilterConventionDefinition
    {
        public IList<TryCreateImplicitFilter> ImplicitFilters { get; }
            = new List<TryCreateImplicitFilter>();

        public IDictionary<FilterKind, ISet<FilterOperationKind>> AllowedOperations { get; }
            = new Dictionary<FilterKind, ISet<FilterOperationKind>>();

        public IDictionary<FilterKind, FilterConventionTypeDefinition> TypeDefinitions
        { get; } = new Dictionary<FilterKind, FilterConventionTypeDefinition>();

        public IDictionary<FilterOperationKind, CreateFieldName> DefaultOperationNames
        { get; } = new Dictionary<FilterOperationKind, CreateFieldName>();

        public IDictionary<FilterOperationKind, NameString> DefaultOperationDescriptions
        { get; } = new Dictionary<FilterOperationKind, NameString>();

        public NameString ArgumentName { get; set; }

        public NameString ArrayFilterPropertyName { get; set; }

        public GetFilterTypeName GetFilterTypeName { get; set; }

    }
}