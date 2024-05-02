import { useState, useEffect } from 'react';
import CartService from './CartService';

import useAuth from './useAuth';

const useCart = () => {
  const [cart, setCart] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const { isAuthenticated } = useAuth();

  const cartService = new CartService();

  const fetchCart = async () => {
    try {
      const data = await cartService.getCart();
      console.log(data);
      setCart(data);
    } catch (err) {
      setError(err);
    } finally {
      setLoading(false);
    }
  };

  const addToCart = async (productId, quantity) => {
    try {
      await cartService.addToCart(productId, quantity);
      const data = await cartService.getCart();
      setCart(data);
    } catch (err) {
      setError(err);
    }
  };

  const updateCart = async (productId, quantity) => {
    try {
      await cartService.updateCart(productId, quantity);

      const data = await cartService.getCart();
      setCart(data);
    } catch (err) {
      setError(err);
    }
  };

  const removeFromCart = async (productId) => {
    try {
      await cartService.removeFromCart(productId);
      const data = await cartService.getCart();
      setCart(data);
    } catch (err) {
      setError(err);
    }
  };

  useEffect(() => {
    if (isAuthenticated === undefined) return;
    fetchCart();
  }, [isAuthenticated]);

  return { cart, loading, error, addToCart, updateCart, removeFromCart };
};

export default useCart;
