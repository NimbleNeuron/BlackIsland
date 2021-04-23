namespace Tftp.Net
{
	
	internal interface ITftpCommandVisitor
	{
		
		void OnReadRequest(ReadRequest command);

		
		void OnWriteRequest(WriteRequest command);

		
		void OnData(Data command);

		
		void OnAcknowledgement(Acknowledgement command);

		
		void OnError(Error command);

		
		void OnOptionAcknowledgement(OptionAcknowledgement command);
	}
}
