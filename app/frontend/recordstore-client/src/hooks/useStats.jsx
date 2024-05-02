import StatsService from './StatsService';

import { useState, useEffect } from 'react';

export const useStats = () => {
  const [financialStats, setFinancialStats] = useState([]);
  const [financialChartStats, setFinancialChartStats] = useState([]);
  const [orderChartStats, setOrderChartStats] = useState([]);
  const [quantityChartStats, setQuantityChartStats] = useState([]);
  const [averageOrderValueChartStats, setAverageOrderValueChartStats] =
    useState([]);
  const [ordersPerRegion, setOrdersPerRegion] = useState([]);

  const [period, setPeriod] = useState('week');

  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const statsService = new StatsService();

  const fetchFinancialStats = async () => {
    try {
      const data = await statsService.getFinancialStats();
      setFinancialStats(data);
    } catch (err) {
      setError(err);
    } finally {
      setLoading(false);
    }
  };

  const fetchFinancialChartStats = async (period) => {
    try {
      const data = await statsService.getFinancialChartStats(period);
      setFinancialChartStats(data);
    } catch (err) {
      setError(err);
    } finally {
      setLoading(false);
    }
  };

  const fetchOrderChartStats = async (period) => {
    try {
      const data = await statsService.getOrderChartStats(period);
      setOrderChartStats(data);

      setQuantityChartStats([]);
    } catch (err) {
      setError(err);
    } finally {
      setLoading(false);
    }
  };

  const fetchOrderChartStatsById = async (id, period) => {
    console.log(id, period);
    if (!id) {
      return await fetchOrderChartStats(period);
    }
    try {
      const data = await statsService.getOrderChartStatsById(id, period);
      setOrderChartStats(data);
    } catch (err) {
      setError(err);
    } finally {
      setLoading(false);
    }
  };

  const fetchQuantityChartStats = async (id, period) => {
    try {
      const data = await statsService.getQuantityChartStats(id, period);
      setQuantityChartStats(data);
    } catch (err) {
      setError(err);
    } finally {
      setLoading(false);
    }
  };

  const fetchAverageOrderValueChartStats = async (period) => {
    try {
      const data = await statsService.getAverageOrderValueChartStats(period);
      setAverageOrderValueChartStats(data);
    } catch (err) {
      setError(err);
    } finally {
      setLoading(false);
    }
  };

  const fetchOrdersPerRegion = async () => {
    try {
      const data = await statsService.getOrdersPerRegion();
      setOrdersPerRegion(data);
    } catch (err) {
      setError(err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchFinancialStats();
    fetchOrdersPerRegion();
  }, []);

  useEffect(() => {
    fetchFinancialChartStats(period);
    fetchOrderChartStats(period);
    fetchAverageOrderValueChartStats(period);
  }, [period]);

  return {
    financialStats,
    financialChartStats,
    orderChartStats,
    loading,
    error,
    setPeriod,
    period,
    fetchOrderChartStatsById,
    fetchQuantityChartStats,
    fetchAverageOrderValueChartStats,
    quantityChartStats,
    averageOrderValueChartStats,
    fetchOrderChartStats,
    ordersPerRegion,
  };
};
