import { useState, useEffect } from 'react';
import OrderService from './OrderService';

export default function useOrders(type = 'user') {
  const [orders, setOrders] = useState(null);
  const [statusOptions, setStatusOptions] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchStatusOptions();
    fetchOrders();
  }, []);

  async function fetchOrders(params) {
    let orderData;
    if (type === 'user') {
      orderData = await OrderService.getOrders(params); // Fetch orders for a specific user
    } else {
      orderData = await OrderService.getAllOrders(params); // Fetch all orders for admin/employee
    }

    setOrders(orderData.data);
    setLoading(false);
  }

  async function fetchStatusOptions() {
    const statusData = await OrderService.getStatusOptions();
    setStatusOptions(statusData.data);
  }

  const createOrder = async (address) => {
    if (type === 'user') {
      return await OrderService.createOrder(address);
    } else {
      throw new Error('Unauthorized: Only customers can create orders.');
    }
  };

  const updateOrderStatus = async (orderId, status) => {
    if (type !== 'employee' && type !== 'admin') {
      throw new Error('Unauthorized: Only employees can update order status.');
    }
    const response = await OrderService.updateOrderStatus(orderId, status);

    if (response.success === true) {
      const order = response.data;
      setOrders((prevOrders) => {
        const index = prevOrders.results.findIndex((o) => o.id === order.id);
        if (index === -1) {
          return prevOrders;
        }
        const newOrders = [...prevOrders.results];
        newOrders[index] = order;
        return { ...prevOrders, results: newOrders };
      });
      return response;
    }
  };

  return {
    orders,
    statusOptions,
    loading,
    createOrder,
    fetchOrders,
    updateOrderStatus,
  };
}
