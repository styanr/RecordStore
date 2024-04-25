import StatsService from './StatsService';

import { useState, useEffect } from 'react';

export const useStats = () => {
  const [financialStats, setFinancialStats] = useState([]);
  const [financialChartStats, setFinancialChartStats] = useState([]);
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

  useEffect(() => {
    fetchFinancialStats();
    fetchFinancialChartStats(period);
  }, []);

  useEffect(() => {
    fetchFinancialChartStats(period);
  }, [period]);

  return {
    financialStats,
    financialChartStats,
    loading,
    error,
    setPeriod,
    period,
  };
};
