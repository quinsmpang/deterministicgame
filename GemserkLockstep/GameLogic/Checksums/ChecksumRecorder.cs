using System.Collections.Generic;

namespace Gemserk.Lockstep 
{
	public class ChecksumRecorder
	{
		readonly ChecksumProvider _checksumProvider;

		readonly List<StoredChecksum> _storedChecksums = new List<StoredChecksum>();

		public List<StoredChecksum> StoredChecksums {
			get {
				return _storedChecksums;
			}
		}

		public ChecksumProvider ChecksumProvider {
			get {
				return _checksumProvider;
			}
		}

		public ChecksumRecorder(ChecksumProvider checksumProvider)
		{
			_checksumProvider = checksumProvider;
		}

		public void Reset()
		{
			_storedChecksums.Clear ();
		}

		public void RecordState (int frame)
		{
			_storedChecksums.Add (new StoredChecksum () {
				gameFrame = frame,
				checksum = _checksumProvider.CalculateChecksum ()
			});
		}
	}
}