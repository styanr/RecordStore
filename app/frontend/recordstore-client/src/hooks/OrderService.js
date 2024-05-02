import axios from 'axios';

const api_url = import.meta.env.VITE_API_URL + 'orders';
const admin_api_url = import.meta.env.VITE_API_URL + 'admin/orders';

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

  async getOrders(params) {
    try {
      const response = await axios.get(api_url, { params });
      return { success: true, data: response.data };
    } catch (error) {
      console.error('Error fetching orders:', error);
      return { success: false, error: error.message };
    }
  }

  async getAllOrders(params) {
    try {
      const response = await axios.get(admin_api_url, { params });
      return { success: true, data: response.data };
    } catch (error) {
      console.error('Error fetching orders:', error);
      return { success: false, error: error.message };
    }
  }

  async updateOrderStatus(orderId, status) {
    try {
      const response = await axios.patch(admin_api_url + `/${orderId}/status`, {
        ...status,
      });
      return { success: true, data: response.data };
    } catch (error) {
      console.error('Error updating order status:', error);
      return {
        success: false,
        error: error.response.data.message || error.message,
      };
    }
  }

  async getStatusOptions() {
    try {
      const response = await axios.get(api_url + '/statuses');
      return { success: true, data: response.data };
    } catch (error) {
      console.error('Error fetching order status options:', error);
      return { success: false, error: error.message };
    }
  }

  async exportOrders(params) {
    const response = await axios.get(admin_api_url + '/report', {
      params,
    });

    return response;
  }
}
export default new OrderService();
