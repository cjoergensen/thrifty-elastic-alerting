using ThriftyElasticAlerting.Model;

namespace ThriftyElasticAlerting.Repositories;

public interface IAlertRepository
{
    Task<IEnumerable<Alert>> GetAll();
}
