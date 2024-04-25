import { useState, useEffect } from 'react';

import OrderService from './OrderService';

export default function useOrders() {
  const [orders, setOrders] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchOrders();
  }, []);

  const createOrder = async (address) => {
    return await OrderService.createOrder(address);
  };

  async function fetchOrders(params) {
    const { data } = await OrderService.getOrders(params);
    setOrders(data);
    setLoading(false);
  }

  return { orders, loading, createOrder, fetchOrders };
}
