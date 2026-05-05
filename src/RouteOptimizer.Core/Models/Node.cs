namespace RouteOptimizer.Core.Models;

public sealed class Node
{
    #region Fields
    public string Id { get; }
    #endregion

    #region Constructors
    public Node(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Node id cannot be empty.", nameof(id));

        Id = id;
    }
    #endregion

    #region Equality
    public bool Equals(Node? other)
    {
        if (other is null)
        {
            return false;
        }
        return string.Equals(Id, other.Id, StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object? obj)
    {
        return obj is Node other && Equals(other);
    }

    public override int GetHashCode()
    {
        return StringComparer.OrdinalIgnoreCase.GetHashCode(Id);
    }

    public static bool operator ==(Node? left, Node? right)
    {
        return EqualityComparer<Node>.Default.Equals(left, right);
    }

    public static bool operator !=(Node? left, Node? right)
    {
        return !(left == right);
    }
    #endregion
}