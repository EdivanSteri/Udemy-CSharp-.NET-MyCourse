namespace MyCourse.Models.Exceptions
{
    public class InvalidVoteException : Exception
    {
        public InvalidVoteException(int vote) : base($"Il voto {vote} non è valido")
        {
        }
    }
}
