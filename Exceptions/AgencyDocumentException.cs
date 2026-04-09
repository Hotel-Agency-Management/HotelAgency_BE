using System.Net;

namespace Booking.Exceptions
{
    public class AgencyDocumentException : AppException
    {
        public AgencyDocumentException(string message, int statusCode)
            : base(message, statusCode) { }
    }

    public class AgencyDocumentNotFoundException : AgencyDocumentException
    {
        public AgencyDocumentNotFoundException(int documentId)
            : base($"Document with id '{documentId}' was not found.", (int)HttpStatusCode.NotFound) { }
    }

    public class AgencyDocumentUploadFailedException : AgencyDocumentException
    {
        public AgencyDocumentUploadFailedException()
            : base("Failed to upload the document.", (int)HttpStatusCode.InternalServerError) { }
    }

    public class AgencyDocumentDeleteFailedException : AgencyDocumentException
    {
        public AgencyDocumentDeleteFailedException()
            : base("Failed to delete the previous document.", (int)HttpStatusCode.InternalServerError) { }
    }
}
