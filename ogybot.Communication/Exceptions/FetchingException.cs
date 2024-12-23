using ogybot.Communication.Constants;

namespace ogybot.Communication.Exceptions;

public class FetchingException(string message) : OgybotException(message)
{
}