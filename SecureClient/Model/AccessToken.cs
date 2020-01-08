namespace SecureClient.Model
{
    public class AccessToken
    {
        public string Token { get; set; }

        public int ExpiryDurationSecs { get; set; }
    }
}
