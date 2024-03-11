namespace Pixel.Domain.Core.Visits.Ports;

using System.Threading.Tasks;
using Pixel.Domain.Core.Visits.Entities;

public interface IVisitRecorder
{
    Task RecordVisit(Visit visit);
}