import React from 'react';
import { ChakraProvider } from '@chakra-ui/react';
import { createBrowserRouter, RouterProvider, Outlet } from 'react-router-dom';
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
        element: <Order />,
      },
    ],
  },
]);

const App = () => {
  return (
    <div>
      <ChakraProvider>
        <RouterProvider router={router} />
      </ChakraProvider>
    </div>
  );
};

export default App;
