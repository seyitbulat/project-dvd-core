

using Core.DiscModule;

namespace Core.ComputerModule.Models;


public enum OpticalDiscDriverStatus
{
  Empty,
  Burning,
  Loaded,
};


public class OpticalDiscDriver
{
  private OpticalDiscDriverStatus _status;
  public OpticalDiscDriverStatus Status => _status;
  private DiscKey? discKey;


  public bool Insert(DiscKey _discKey)
  {
    switch (_status)
    {
      case OpticalDiscDriverStatus.Empty:

      return true;
      default:
      return false;
    }
  }
  public bool Eject()
  {
    switch (_status)
    {
      case OpticalDiscDriverStatus.Burning:
        return false;
      case OpticalDiscDriverStatus.Empty:
        return false;
      default:
        return true;
    }
  }

}