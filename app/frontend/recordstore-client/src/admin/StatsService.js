import axios from 'axios';

const api_url = import.meta.env.VITE_API_URL + 'admin/stats';

class StatsService {
  getFinancialStats = async () => {
    const response = await axios.get(api_url + '/financial');
    return response.data;
  };

  getFinancialChartStats = async (period) => {
    const response = await axios.get(api_url + '/financial/' + period);
    return response.data;
  };
}

export default StatsService;
