namespace TournamentAPI.Services;

public class RateLimitingService
{
    private readonly Dictionary<string, Queue<DateTime>> _requestHistory = new();
    private readonly int _maxRequests = 2; // 2 requests
    private readonly int _timeWindowSeconds = 1; // per second
    private readonly object _lockObject = new();

    public bool IsRequestAllowed(string clientId)
    {
        lock (_lockObject)
        {
            var now = DateTime.UtcNow;

            if (!_requestHistory.ContainsKey(clientId))
            {
                _requestHistory[clientId] = new Queue<DateTime>();
            }

            var queue = _requestHistory[clientId];

            // Remove old requests outside the time window
            while (queue.Count > 0 && (now - queue.Peek()).TotalSeconds > _timeWindowSeconds)
            {
                queue.Dequeue();
            }

            // Check if limit is exceeded
            if (queue.Count >= _maxRequests)
            {
                return false;
            }

            // Add current request
            queue.Enqueue(now);
            return true;
        }
    }
}
