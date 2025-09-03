using WinFormsApp2;

namespace WinFormsApp2
{
    /// <summary>
    /// Unity stub for TavernService; provides no-op notification methods
    /// so UI scripts can compile without the server-side implementation.
    /// </summary>
    public static class TavernService
    {
        public static void NotifyPartyHired(int ownerId) { }
        public static void NotifyPartyReturned(int ownerId) { }
    }
}
