namespace Adobe.Target.Client.Model
{
    using System.Collections.Generic;
    using System.Net;
    using Adobe.Target.Client.Util;
    using Adobe.Target.Delivery.Model;

    /// <summary>
    /// Target Delivery Response
    /// </summary>
    public class TargetDeliveryResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TargetDeliveryResponse"/> class.
        /// </summary>
        /// <param name="request">Request</param>
        /// <param name="response">Response</param>
        /// <param name="status">Status</param>
        /// <param name="message">Message</param>
        public TargetDeliveryResponse(TargetDeliveryRequest request, DeliveryResponse response, HttpStatusCode status, string message)
        {
            this.Request = request;
            this.Response = response;
            this.Status = status;
            this.Message = message;
        }

        /// <summary>
        /// Target Delivery request
        /// </summary>
        public TargetDeliveryRequest Request { get; }

        /// <summary>
        /// Delivery response
        /// </summary>
        public DeliveryResponse Response { get; }

        /// <summary>
        /// Status
        /// </summary>
        public HttpStatusCode Status { get; }

        /// <summary>
        /// Message
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Gets Target cookies
        /// </summary>
        /// <returns>Target cookies</returns>
        public Dictionary<string, TargetCookie> GetCookies()
        {
            var cookies = new Dictionary<string, TargetCookie>();

            if (this.Response == null || this.Response.Status < (int)HttpStatusCode.OK ||
                this.Response.Status >= (int)HttpStatusCode.Ambiguous)
            {
                return cookies;
            }

            var targetCookie = CookieUtils.CreateTargetCookie(this.Request.SessionId, this.Response.Id.TntId);
            if (targetCookie != null)
            {
                cookies.Add(TargetConstants.MboxCookieName, targetCookie);
            }

            var clusterCookie = CookieUtils.CreateClusterCookie(this.Response.Id.TntId);
            if (clusterCookie != null)
            {
                cookies.Add(TargetConstants.ClusterCookieName, clusterCookie);
            }

            return cookies;
        }
    }
}
