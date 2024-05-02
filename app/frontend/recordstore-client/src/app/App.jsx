import React, { useEffect } from 'react';
import { Box, Flex, ChakraProvider } from '@chakra-ui/react';
import {
  createBrowserRouter,
  RouterProvider,
  Outlet,
  Navigate,
} from 'react-router-dom';

import { AuthProvider } from '../AuthContext';

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
import OrdersManage from '../employee/employee-orders/OrdersManage';
import PurchaseOrders from '../employee/purchase-orders/PurchaseOrders';
import CreatePurchaseOrder from '../employee/purchase-orders/new/CreatePurchaseOrder';
import Artists from '../employee/employee-artists/Artists';
import Artist from '../artist/Artist';

import useAuth from '../hooks/useAuth';
import CreateProduct from '../employee/products/CreateProduct';

import Records from '../employee/records/new/Records';
import CreateRecord from '../employee/records/new/CreateRecord';
import EditRecord from '../employee/records/new/EditRecord';
import CreateUser from '../admin/CreateUser';
import Register from '../auth/Register';

const ProtectedUserRoute = ({ element }) => {
  const { user, isAuthenticated, loading } = useAuth();

  useEffect(() => {
    console.log('ProtectedUserRoute');
  }, []);

  // If authentication state is still being determined, show a loading indicator
  if (isAuthenticated === undefined || loading) {
    console.log(isAuthenticated, loading);
    return <div>Loading...</div>;
  }

  // If the user is not authenticated, allow access
  if (!isAuthenticated) {
    return element;
  }

  if (user && user.role === 'employee') {
    return <Navigate to='/orders-manage' />;
  }

  // If the user is authenticated and not an admin, allow access
  if (user && user.role === 'admin') {
    return <Navigate to='/dashboard' />;
  }

  return element;
};

const ProtectedEmployeeRoute = ({ element }) => {
  const { user, isAuthenticated, loading } = useAuth();

  useEffect(() => {
    console.log('ProtectedEmployeeRoute');
  }, []);

  if (loading) {
    return <div>Loading...</div>;
  }

  if (
    isAuthenticated &&
    user &&
    (user.role === 'employee' || user.role === 'admin')
  ) {
    return element;
  }

  return <Navigate to='/' />;
};

const App = () => {
  const router = createBrowserRouter([
    {
      element: (
        <AuthProvider>
          <Flex direction='column' minH='100vh'>
            <Simple />
            <Box mt={16} flexGrow={1} display={'flex'} flexDir={'column'}>
              <Outlet />
            </Box>
          </Flex>
        </AuthProvider>
      ),
      children: [
        {
          path: '/',
          element: <ProtectedUserRoute element={<Home />} />,
        },
        {
          path: '/register',
          element: <Register />,
        },
        {
          path: '/login',
          element: <Login />,
        },
        {
          path: '/products',
          element: <Home />,
        },
        {
          path: '/products/new',
          element: <ProtectedEmployeeRoute element={<CreateProduct />} />,
        },
        {
          path: '/products/:id',
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
          path: '/orders-manage',
          element: <ProtectedEmployeeRoute element={<OrdersManage />} />,
        },
        {
          path: '/purchase-orders',
          element: <ProtectedEmployeeRoute element={<PurchaseOrders />} />,
        },
        {
          path: '/purchase-orders/new',
          element: <ProtectedEmployeeRoute element={<CreatePurchaseOrder />} />,
        },
        {
          path: '/artists',
          element: <ProtectedEmployeeRoute element={<Artists />} />,
        },
        {
          path: '/artists/:id',
          element: <Artist />,
        },
        {
          path: '/records',
          element: <ProtectedEmployeeRoute element={<Records />} />,
        },
        {
          path: '/records/new',
          element: <ProtectedEmployeeRoute element={<CreateRecord />} />,
        },
        {
          path: '/records/:id/edit',
          element: <ProtectedEmployeeRoute element={<EditRecord />} />,
        },
        {
          path: '/users/new',
          element: <ProtectedAdminRoute element={<CreateUser />} />,
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
