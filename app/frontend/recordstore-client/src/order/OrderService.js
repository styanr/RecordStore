import axios from 'axios';

const api_url = import.meta.env.VITE_API_URL + 'orders';

class OrderService {
  async createOrder(address) {
    try {
      const response = await axios.post(api_url, address);
      return { success: true, data: response.data };
    } catch (error) {
      console.error('Error creating order:', error);
      return { success: false, error: error.message };
    }
  }

  getOrders(params) {
    return axios.get(api_url, { params });
  }
}

export default new OrderService();
