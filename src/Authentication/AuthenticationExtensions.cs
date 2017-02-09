using Nancy;

namespace Cimpress.Nancy.Authentication
{
    public static class AuthenticationExtensions
    {
        public static IAuthVerifier Verifier;

        public static void RequiresCustomAuthentication(this NancyModule module)
        {
            module.Before.AddItemToEndOfPipeline(PreCheck);
            module.After.AddItemToEndOfPipeline(PostCheck);
        }

        private static Response PreCheck(NancyContext context)
        {
            return Verifier.HandleBefore(context);
        }

        private static void PostCheck(NancyContext context)
        {
            Verifier.HandleAfter(context);
        }
    }
}