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

  getOrderChartStats = async (period) => {
    const response = await axios.get(api_url + '/orders/' + period);
    return response.data;
  };

  getOrderChartStatsById = async (id, period) => {
    const response = await axios.get(api_url + '/orders/' + id + '/' + period);
    return response.data;
  };

  getQuantityChartStats = async (id, period) => {
    const response = await axios.get(
      api_url + '/products/' + id + '/quantity-sold/' + period
    );
    return response.data;
  };

  getAverageOrderValueChartStats = async (period) => {
    const response = await axios.get(
      api_url + '/average-order-value/' + period
    );
    return response.data;
  };

  getOrdersPerRegion = async () => {
    const response = await axios.get(api_url + '/orders-per-region');
    return response.data;
  };
}

export default StatsService;
