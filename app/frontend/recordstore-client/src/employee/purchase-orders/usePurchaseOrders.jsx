import axios from 'axios';
import { useState, useEffect } from 'react';

const api_url = import.meta.env.VITE_API_URL;

const usePurchaseOrders = (purchaseOrderParams = {}, supplierParams = {}) => {
  const [purchaseOrders, setPurchaseOrders] = useState([]);
  const [isPurchaseOrderLoading, setIsPurchaseOrderLoading] = useState(true);
  const [purchaseOrderError, setPurchaseOrderError] = useState(null);

  const [suppliers, setSuppliers] = useState([]);
  const [isSupplierLoading, setIsSupplierLoading] = useState(false);
  const [supplierError, setSupplierError] = useState(null);

  useEffect(() => {
    const fetchPurchaseOrders = async () => {
      setIsPurchaseOrderLoading(true);
      setPurchaseOrderError(null);
      try {
        const response = await axios.get(`${api_url}purchase-orders`, {
          params: purchaseOrderParams,
        });
        setPurchaseOrders(response.data);
      } catch (error) {
        setPurchaseOrderError(error);
      } finally {
        setIsPurchaseOrderLoading(false);
      }
    };

    fetchPurchaseOrders();
  }, [purchaseOrderParams]);

  useEffect(() => {
    console.log(supplierParams);
    const fetchSuppliers = async () => {
      setIsSupplierLoading(true);
      setSupplierError(null);
      try {
        const response = await axios.get(
          `${api_url}purchase-orders/suppliers`,
          {
            params: supplierParams,
          }
        );
        setSuppliers(response.data);
      } catch (error) {
        setSupplierError(error);
      } finally {
        setIsSupplierLoading(false);
      }
    };

    fetchSuppliers();
  }, [supplierParams]);

  const createPurchaseOrder = async (purchaseOrder) => {
    try {
      await axios.post(`${api_url}purchase-orders`, purchaseOrder);
      return { success: true };
    } catch (error) {
      console.error('Error creating purchase order:', error);
      return { success: false, message: error.message };
    }
  };

  const deletePurchaseOrder = async (purchaseOrderId) => {
    try {
      await axios.delete(`${api_url}purchase-orders/${purchaseOrderId}`);
      setPurchaseOrders(
        purchaseOrders.filter((order) => order.id !== purchaseOrderId)
      );
      return { success: true };
    } catch (error) {
      console.error('Error deleting purchase order:', error);
      return { success: false, message: error.message };
    }
  };

  return {
    purchaseOrders,
    isPurchaseOrderLoading,
    purchaseOrderError,
    suppliers,
    isSupplierLoading,
    supplierError,
    createPurchaseOrder,
    deletePurchaseOrder,
  };
};

export default usePurchaseOrders;
