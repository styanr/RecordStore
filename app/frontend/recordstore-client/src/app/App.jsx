import React, { useEffect } from 'react';
import { ChakraProvider } from '@chakra-ui/react';
import {
  createBrowserRouter,
  RouterProvider,
  Outlet,
  Navigate,
} from 'react-router-dom';
import Simple from '../common/NavBar';
import Login from '../auth/Login';
import Home from '../home/Home';
import Product from '../product/Product';
import Profile from '../profile/Profile';
import ProtectedRoute from '../common/ProtectedRoute';
import ProtectedAdminRoute from '../common/ProtectedAdminRoute';
import Dashboard from '../admin/Dashboard';
import Logout from '../common/Logout';
import Order from '../order/Order';
import OrdersManage from '../employee-orders/OrdersManage';

import useAuth from '../auth/useAuth';

const ProtectedUserRoute = ({ element }) => {
  const { user, isAuthenticated } = useAuth();

  // If authentication state is still being determined, show a loading indicator
  if (isAuthenticated === undefined) {
    return <div>Loading...</div>;
  }

  // If the user is not authenticated, allow access
  if (!isAuthenticated) {
    return element;
  }

  if (user && user.role === 'employee') {
    return <Navigate to='/employee-dashboard' />;
  }

  // If the user is authenticated and not an admin, allow access
  if (user && user.role !== 'admin') {
    return element;
  }

  // If the user is authenticated and is an admin, redirect to the dashboard
  return <Navigate to='/dashboard' />;
};

const ProtectedEmployeeRoute = ({ element }) => {
  const { user, isAuthenticated } = useAuth();

  if (isAuthenticated === undefined) {
    return <div>Loading...</div>;
  }

  if (!isAuthenticated) {
    return <Navigate to='/login' />;
  }

  if (user && user.role === 'employee') {
    return element;
  }

  return <Navigate to='/' />;
};

const App = () => {
  const router = createBrowserRouter([
    {
      element: (
        <>
          <Simple />
          <Outlet />
        </>
      ),
      children: [
        {
          path: '/',
          element: <ProtectedUserRoute element={<Home />} />,
        },
        {
          path: '/products',
          element: <Home />,
        },
        {
          path: '/login',
          element: <Login />,
        },
        {
          path: '/product/:id',
          element: <Product />,
        },
        {
          path: '/me',
          element: <ProtectedRoute element={<Profile />} />,
        },
        {
          path: '/dashboard',
          element: <ProtectedAdminRoute element={<Dashboard />} />,
        },
        {
          path: '/logout',
          element: <Logout />,
        },
        {
          path: '/order',
          element: <ProtectedUserRoute element={<Order />} />,
        },
        {
          path: '/employee-dashboard',
          element: <ProtectedEmployeeRoute element={<OrdersManage />} />,
        },
      ],
    },
  ]);
  return (
    <div>
      <ChakraProvider>
        <RouterProvider router={router} />
      </ChakraProvider>
    </div>
  );
};

export default App;
