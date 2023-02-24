using Microsoft.SqlServer.TransactSql.ScriptDom;
using System.Reflection;
using System.Text;

namespace ScriptHaqonizer.Playground;

internal class MicrosoftSqlServerTransactSqlScriptDom
{
    public static void Test()
    {
        const string sql = @"
SELECT * FROM Tables AS t
INNER JOIN OMG as o ON o.ID = t.ID
";
        var parser = new TSql160Parser(true, SqlEngineType.All);
        var root = parser.Parse(new StringReader(sql), out var errors);
        var tree = MakeTree(root, null);
        Console.WriteLine(tree);
    }

    public static TSqlNode MakeTree(TSqlFragment fragment, TSqlNode? parent)
    {
        var baseType = typeof(TSqlFragment);
        var allProps = fragment.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
        var props = allProps.Where(p => p.PropertyType.IsAssignableTo(baseType));

        var thisNode = new TSqlNode(fragment, parent, new List<TSqlNode>());
        foreach (var prop in allProps)
        {
            if (prop.PropertyType.IsAssignableTo(baseType) && prop.GetIndexParameters().Length == 0)
            {
                var value = (TSqlFragment?) prop.GetValue(fragment);
                if (value is not null)
                {
                    var node = MakeTree(value, thisNode);
                    thisNode.Children.Add(node);
                }
            }
            else if (prop.PropertyType.IsAssignableTo(typeof(IEnumerable<>).MakeGenericType(baseType)))
            {
                var value = (IEnumerable<TSqlFragment>?) prop.GetValue(fragment);
                if (value is not null)
                {
                    foreach (var v in value)
                    {
                        var node = MakeTree(v, thisNode);
                        thisNode.Children.Add(node);
                    }
                }
            }
        }

        return thisNode;
    }

    public record TSqlNode(TSqlFragment Fragment, TSqlNode? Parent, List<TSqlNode> Children)
    {
        public override string ToString()
        {
            var sb = new StringBuilder();
            FormatTo(sb, 0);
            return sb.ToString();
        }

        private void FormatTo(StringBuilder builder, int level)
        {
            builder.Append('\t', level);
            builder.Append(Fragment.GetType().Name);
            builder.AppendLine();

            foreach (var node in Children)
            {
                node.FormatTo(builder, level + 1);
            }
        }
    };
}