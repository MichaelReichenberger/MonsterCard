namespace MonsterCardTradingGame.Server.Routes
{
    public class Route
    {
        public string Method { get; }
        public string Url { get; }
        private readonly RouteAction _action;
        private readonly AsyncRouteAction _asyncAction;
        private readonly bool _isAsync;

        public Route(string method, string url, RouteAction action)
        {
            Method = method;
            Url = url;
            _action = action;
            _isAsync = false;
        }

        public Route(string method, string url, AsyncRouteAction action)
        {
            Method = method;
            Url = url;
            _asyncAction = action;
            _isAsync = true;
        }

        public async Task<string> Execute(string requestBody, string requestParameter, int userId)
        {
            return _isAsync ? await _asyncAction(requestBody, requestParameter, userId)
                : _action(requestBody, requestParameter, userId);
        }
    }
}