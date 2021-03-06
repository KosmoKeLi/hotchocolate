namespace HotChocolate.Language.Visitors
{
    public partial class SyntaxVisitor<TContext>
        : ISyntaxVisitor<TContext>
        where TContext : ISyntaxVisitorContext
    {
        private readonly SyntaxVisitorOptions _options;

        public SyntaxVisitor(SyntaxVisitorOptions options = default)
        {
            DefaultAction = Skip;
            _options = options;
        }

        public SyntaxVisitor(
            ISyntaxVisitorAction defaultResult,
            SyntaxVisitorOptions options = default)
        {
            DefaultAction = defaultResult;
            _options = options;
        }

        protected virtual ISyntaxVisitorAction DefaultAction { get; }

        /// <summary>
        /// Ends traversing the graph.
        /// </summary>
        public static ISyntaxVisitorAction Break { get; } = new BreakSyntaxVisitorAction();

        /// <summary>
        /// Skips of the child nodes.
        /// </summary>
        public static ISyntaxVisitorAction Skip { get; } = new SkipSyntaxVisitorAction();

        /// <summary>
        /// Continues traversing the graph.
        /// </summary>
        public static ISyntaxVisitorAction Continue { get; } = new ContinueSyntaxVisitorAction();

        public ISyntaxVisitorAction Visit(
            ISyntaxNode node,
            TContext context) =>
            Visit<ISyntaxNode, ISyntaxNode?>(node, null, context);

        protected ISyntaxVisitorAction Visit<T, P>(
            T node,
            P parent,
            TContext context)
            where T : notnull, ISyntaxNode
            where P : ISyntaxNode?
        {
            var localContext = OnBeforeEnter(node, parent, context);
            var result = Enter(node, localContext);
            localContext = OnAfterEnter(node, parent, localContext);

            if (result.Kind == SyntaxVisitorActionKind.Break)
            {
                return result;
            }

            if (result.Kind == SyntaxVisitorActionKind.Continue)
            {
                VisitChildren(node, context);
            }

            localContext = OnBeforeLeave(node, parent, localContext);
            result = Leave(node, localContext);
            localContext = OnAfterLeave(node, parent, localContext);

            return result;
        }

        protected virtual ISyntaxVisitorAction Enter(
            ISyntaxNode node,
            TContext context) =>
            DefaultAction;

        protected virtual ISyntaxVisitorAction Leave(
            ISyntaxNode node,
            TContext context) =>
            DefaultAction;

        protected virtual TContext OnBeforeEnter(
            ISyntaxNode node,
            ISyntaxNode? parent,
            TContext context) =>
            context;

        protected virtual TContext OnAfterEnter(
            ISyntaxNode node,
            ISyntaxNode? parent,
            TContext context) =>
            context;

        protected virtual TContext OnBeforeLeave(
            ISyntaxNode node,
            ISyntaxNode? parent,
            TContext context) =>
            context;

        protected virtual TContext OnAfterLeave(
            ISyntaxNode node,
            ISyntaxNode? parent,
            TContext context) =>
            context;
    }
}
